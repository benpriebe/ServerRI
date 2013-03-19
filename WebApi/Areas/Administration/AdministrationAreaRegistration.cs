using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApi.Areas.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public const string ProductsRouteName = "Products";
        public const string ProductsSoldOutRouteName = "ProductsSoldOut";

        public override string AreaName
        {
            get
            {
                return "Administration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RegisterRoutes(context.Routes);
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
              name: AreaName + "/" + ProductsSoldOutRouteName,
              routeTemplate: "api/admin/products/{id}/mark-sold-out",
              defaults: new
              {
                  controller = "ProductsAdmin",
                  action = "MarkSoldOut"
              });

            routes.MapHttpRoute(
                  name: AreaName + "/" + ProductsRouteName,
                  routeTemplate: "api/admin/products/{id}",
                  defaults: new
                  {
                      controller = "ProductsAdmin",
                      id = RouteParameter.Optional,
                      action = "Default"
                  });
        }
    }
}
