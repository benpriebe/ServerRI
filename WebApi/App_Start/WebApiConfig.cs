#region Using directives

using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#endregion


namespace WebApi
{
    public static class WebApiConfig
    {
        public const string ProductsRouteName = "Products";
        public const string ProductsWithDetailsRouteName = "ProductsWithDetails";
        public const string ProductsSoldOutRouteName = "ProductsSoldOutRouteName";

        public static void Configure(HttpConfiguration configuration)
        {
            ConfigureFormatters(configuration);
            RegisterRoutes(configuration.Routes);
        }

        private static void ConfigureFormatters(HttpConfiguration configuration)
        {
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
        }

        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: ProductsWithDetailsRouteName,
                routeTemplate: "api/products/details",
                defaults: new
                {
                    controller = "Products",
                    id = RouteParameter.Optional,
                    action = "Details"
                });

            routes.MapHttpRoute(
             name: ProductsSoldOutRouteName,
             routeTemplate: "api/products/{id}/mark-sold-out",
             defaults: new
             {
                 controller = "Products",
                 action = "MarkSoldOut"
             });

            routes.MapHttpRoute(
                name: ProductsRouteName,
                routeTemplate: "api/products/{id}",
                defaults: new
                {
                    controller = "Products", id = RouteParameter.Optional, action = "Default"
                });
        }
    }
}