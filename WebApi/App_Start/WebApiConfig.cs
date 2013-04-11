#region Using directives

using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Routing.Conventions;
using Microsoft.Data.Edm;
using Models.Customers;
using Models.Products;
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

        public const string CustomersRouteName = "Customers";
        public const string CustomerAddressesRouteName = "CustomerAddresses";

        public static void Configure(HttpConfiguration configuration)
        {
            ConfigureFormatters(configuration);
            RegisterRoutes(configuration.Routes);
            ConfigureODataSettings(configuration);
        }

        private static void ConfigureODataSettings(HttpConfiguration configuration)
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<ProductModel>("ProductModels");
            var customers = modelBuilder.EntitySet<CustomerModelResponse>("CustomerModels");
            customers.EntityType.HasKey(c => c.CustomerID);
            
            IEdmModel model = modelBuilder.GetEdmModel();
            
            configuration.Routes.MapODataRoute(routeName: "OData", routePrefix: "odata", model: model);
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

            
            routes.MapHttpRoute(
               name: CustomerAddressesRouteName,
               routeTemplate: "api/customers/{id}/addresses",
               defaults: new
               {
                   controller = "Customers",
                   action = "AddressesForCustomer"
               });

            routes.MapHttpRoute(
                name: CustomersRouteName,
                routeTemplate: "api/customers/{id}",
                defaults: new
                {
                    controller = "Customers",
                    id = RouteParameter.Optional,
                    action = "Default"
                });
        }
    }
}