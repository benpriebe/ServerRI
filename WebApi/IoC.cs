#region Using directives

using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Contracts.Data;
using Data;
using Data.Entities;
using Providers;
using Providers.Administration;
using Services;

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
                .InstancePerHttpRequest()
                .InstancePerApiRequest()
                .Named<DbConnection>("DbConnection");

            builder.Register(c =>
            {
                var connection = c.ResolveNamed<DbConnection>("DbConnection");
                return new AWContext(connection, false);
            })
                .InstancePerHttpRequest()
                .InstancePerApiRequest()
                .As<DbContext>();

            // services
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly).Where(t => t.Name.EndsWith("Service"));

            // providers
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<ProductsProvider>().As<IProductsProvider>();
            builder.RegisterType<EFProvider<Customer>>().As<IProvider<Customer>>();

            Container = builder.Build();
        }
    }
}