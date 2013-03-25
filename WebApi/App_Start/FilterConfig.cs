#region Using directives

using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Autofac;
using Common.Logging;
using Core;
using Core.Extensions;
using Elmah.Contrib.WebApi;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

#endregion


namespace WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());
            using (var container = IoC.Container.BeginLifetimeScope())
            {
                GlobalConfiguration.Configuration.Filters.Add(container.Resolve<CommonLogErrorApiAttribute>());
            }
        }

        public class CommonLogErrorApiAttribute : ExceptionFilterAttribute
        {
            private readonly OperationContext _context;
            private readonly ILog _log;

            public CommonLogErrorApiAttribute(OperationContext context, ILog log)
            {
                _context = context;
                _log = log;
            }

            public override void OnException(HttpActionExecutedContext actionExecutedContext)
            {
                if (actionExecutedContext.Exception != null)
                    _log.Exception(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), _context), actionExecutedContext.Exception);

                base.OnException(actionExecutedContext);
            }
        }
    }
}