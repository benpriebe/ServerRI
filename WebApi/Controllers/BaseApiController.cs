#region Using directives

using System;
using System.Linq;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using Common.Logging;
using Core;

#endregion


namespace WebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        private readonly OperationContext _context;
        private readonly ILog _log;

        public BaseApiController(OperationContext context, ILog log)
        {
            _context = context;
            _log = log;

        }

        public ILog Log
        {
            get { return _log; }
        }

        public OperationContext Context
        {
            get { return _context; }
        }

        public HttpResponseMessage Invoke(HttpRequestMessage request, Func<HttpResponseMessage> func)
        {
            HttpResponseMessage response;
            bool commit = !request.Headers.Any(header => header.Key == "TransactionRollback" && header.Value.Any(val => bool.Parse(val)));

            // this is a failsafe for when users forget to pass in the TrasactionRollback Http Header.
#if DEBUG
            commit = false;
#endif

            if (commit)
            {
                response = func();
            }
            else
            {
                using (new TransactionScope())
                {
                    response = func();
                    // no complete so rollsback
                }
            }
            return response;
        }
    }
}