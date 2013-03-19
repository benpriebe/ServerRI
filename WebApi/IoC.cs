#region Using directives

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;
using System.Web.ModelBinding;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Contracts.Data;
using Core;
using Core.IoCModules;
using Data;
using Data.Entities;
using Models.Administration.Products;
using Providers;
using Providers.Administration;
using Services;
using WebApi.Controllers;
using IModelBinder = System.Web.Http.ModelBinding.IModelBinder;
using ModelBinderProvider = System.Web.Http.ModelBinding.ModelBinderProvider;
using ModelBindingContext = System.Web.Http.ModelBinding.ModelBindingContext;

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
            //builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            
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

           
            //TODO: 18-Mar-2013 - Ben - Figure out how to do this with chose authentication model.
            // operationcontext - should only have one instance per thread/user
            builder.RegisterInstance(new OperationContext(new UserDetails(1.ToString(), "To Do", "to.do")));

            // logging
            builder.RegisterModule<IoCLoggingModule>();

            // filters
            builder.RegisterType<FilterConfig.CommonLogErrorApiAttribute>();
            
            Container = builder.Build();

            // webapi controller resolver
            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(Container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiDependencyResolver;
        }
    }

}