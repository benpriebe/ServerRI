#region Using directives

using System.Web.Mvc;

#endregion


namespace WebApi.Areas.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Administration"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Administration_default",
                "Administration/{controller}/{action}/{id}",
                new
                {
                    action = "Index", id = UrlParameter.Optional
                }
                );
        }
    }
}