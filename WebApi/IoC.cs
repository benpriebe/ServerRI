#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Contracts.Data;
using Core;
using Core.IoCModules;
using Data;
using Data.Entities;
using Providers.Data;
using Providers.External;
using Services;
using WebApi.Resx;

#endregion


namespace WebApi
{
    internal static class IoC
    {
        static IoC()
        {
            BuildContainer();
        }

        internal static IContainer Container { get; set; }

        private static string DbConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["AWContext"].ConnectionString; }
        }

        internal static void BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());

            // database
            builder.Register(c => Database.DefaultConnectionFactory.CreateConnection(DbConnectionString))
                .InstancePerApiRequest()
                .Named<DbConnection>("DbConnection");

            builder.Register(c =>
            {
                var connection = c.ResolveNamed<DbConnection>("DbConnection");
                return new AWContext(connection, false);
            })
                .InstancePerApiRequest()
                .As<DbContext>();

            // services
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly).Where(t => t.Name.EndsWith("Service"));

            // providers
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            builder.RegisterType<ProductsProvider>().As<IProductsProvider>();
            builder.RegisterType<EFProvider<Customer>>().As<IProvider<Customer>>();
            builder.RegisterType<ExternalProvider>().As<IExternalProvider>();

            //TODO: 18-Mar-2013 - Ben - Figure out how to do this with chose authentication model.
            // operationcontext - should only have one instance per thread/user
            builder.RegisterInstance(new OperationContext(new UserDetails(1.ToString(), "To Do", "to.do")));

            // logging
            builder.RegisterModule<IoCLoggingModule>();

            // filters
            builder.RegisterType<FilterConfig.CommonLogErrorApiAttribute>();

            // localized model/validation metadata stuff.
            builder.Register(c => new LocalizationProvider())
                .As<ModelMetadataProvider>();

            Container = builder.Build();

            // webapi controller resolver
            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(Container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiDependencyResolver;
        }
    }

    /// <summary>
    /// Takes the DataAnnotations validation attributes and the DisplayAttribute and localizes them based on a recursive naming structure.
    /// </summary>
    public class LocalizationProvider : DataAnnotationsModelMetadataProvider
    {
        public class Resource
        {
            public ResourceManager ResourceManager { get; set; }
            public Type ResourceType { get; set; }

            public bool Equals(Resource other)
            {
                return Equals(ResourceManager, other.ResourceManager) && Equals(ResourceType, other.ResourceType);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Resource && Equals((Resource) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ResourceManager != null ? ResourceManager.GetHashCode() : 0) * 397) ^ (ResourceType != null ? ResourceType.GetHashCode() : 0);
                }
            }
        }

        private static readonly Dictionary<string, Resource> _resourceManagers = new Dictionary<string, Resource>();

        protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Type modelType,
            string propertyName)
        {
            var metadata = base.CreateMetadataPrototype(attributes, containerType, modelType, propertyName);

            if (containerType == null || propertyName == null)
                return metadata;

            var attrs = attributes.ToList();
            foreach (var attr in attrs)
            {
                var valAttr = attr as ValidationAttribute;
                if (valAttr != null)
                {
                    var key = valAttr.GetType().Name;

                    var strLengthAttr = valAttr as StringLengthAttribute;
                    if (strLengthAttr != null && strLengthAttr.MinimumLength != 0)
                    {
                        key += "_IncludingMinimum";
                    }

                    valAttr.ErrorMessageResourceName = key;
                    valAttr.ErrorMessageResourceType = typeof(ValidationErrors);
                }
                else
                {
                    var attribute = attr as DisplayAttribute; 
                    if (attribute != null)
                    {
                        var resource = GetResource(containerType);
                        var resourceKey = GetResourceKey(resource.ResourceManager, containerType, propertyName);
                        if (!String.IsNullOrWhiteSpace(resourceKey))
                        {
                            attribute.Name = resourceKey;
                            attribute.ResourceType = resource.ResourceType;
                        }
                    }
                }
            }

            var result = base.CreateMetadataPrototype(attrs, containerType, modelType, propertyName);
            return result;
        }

        public static Resource GetResource(Type containerType)
        {
            var resxClassName = containerType.Namespace + ".Resx.Strings";
            Resource resource;
            _resourceManagers.TryGetValue(resxClassName, out resource);

            if (resource != null)
                return resource;
            
            var assembly = Assembly.GetAssembly(containerType);
            var resourceStrings = assembly.GetType(resxClassName);

            var pi = resourceStrings.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static);
            var rm = (ResourceManager) pi.GetMethod.Invoke(resourceStrings, null);
            resource = new Resource { ResourceManager = rm, ResourceType = resourceStrings };
            _resourceManagers.Add(resxClassName, resource);
            return resource;
        }

        /// <summary>
        /// Recursively searches up the tree (base types and interfaces for a resx property value based off the Type passed in.
        /// </summary>
        /// <returns></returns>
        public string GetResourceKey(ResourceManager rm, Type type, string propertyName)
        {
            if (type == null)
                return null;

            var resourceKey = string.Format("{0}_DisplayAttribute_{1}", type.Name, propertyName);
            var resourceValue = rm.GetString(resourceKey);

            if (!String.IsNullOrWhiteSpace(resourceValue))  
                return resourceKey;

            resourceKey = GetResourceKey(rm, type.BaseType, propertyName);
            if (!String.IsNullOrWhiteSpace(resourceValue))
                return resourceKey;

            // note: no need to check interfaces as [DisplayAttribute] doesn't pass down from them.

            return resourceKey;
        }
    }
}