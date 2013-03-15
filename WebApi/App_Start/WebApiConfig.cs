#region Using directives

using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion


namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Filters.Add(new ElmahErrorFilterAttribute());
            config.Filters.Add(new AuthorizeAttribute());

            // Setup JSON settings
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

            RegisterRoutes(config.Routes);
        }

        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
                );
        }
    }
}