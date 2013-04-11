#region Using directives

using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Metadata;
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
using WebApi.Controllers;
using WebApi.Controllers.OData;

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
            builder.RegisterType<ExternalProvider>().As<IExternalProvider>();
            builder.RegisterGeneric(typeof(EFProvider<>)).As(typeof(IProvider<>));
            
            //TODO: 18-Mar-2013 - Ben - Figure out how to do this with chose authentication model.
            // operationcontext - should only have one instance per thread/user
            builder.RegisterInstance(new OperationContext(new UserDetails(1.ToString(), "To Do", "to.do")));

            // logging
            builder.RegisterModule<IoCLoggingModule>();

            // filters
            builder.RegisterType<FilterConfig.CommonLogErrorApiAttribute>();

            // localized model/validation metadata stuff.
            builder.Register(c => new LocalizationProvider<Models.Resx.Strings>(Models.Resx.Strings.ResourceManager)).As<ModelMetadataProvider>();

            // odata controllers
            builder.RegisterType<ProductModelsController>();
            builder.RegisterType<CustomerModelsController>();

            Container = builder.Build();

            // webapi controller resolver
            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(Container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiDependencyResolver;
        }
    }
}