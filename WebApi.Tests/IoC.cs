#region Using directives

using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;

#endregion


namespace WebApi.Tests
{
    internal static class IoC
    {
        static IoC()
        {
            BuildContainer();
        }

        internal static IContainer Container { get; set; }

        internal static void BuildContainer()
        {
            var builder = new ContainerBuilder();

            var webapiAssembly = Assembly.GetAssembly(typeof(WebApiConfig));
            builder.RegisterApiControllers(webapiAssembly);
           
            Container = builder.Build();

        }
    }
}