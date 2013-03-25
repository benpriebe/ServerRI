#region Using directives

using System.Web;
using System.Web.Http;
using System.Web.Mvc;

#endregion


namespace WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            IoC.BuildContainer();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Configure(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}