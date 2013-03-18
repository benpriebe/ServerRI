#region Using directives

using System;
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
        private readonly bool _commit = true;
        private readonly OperationContext _context;
        private readonly ILog _log;

        public BaseApiController(OperationContext context, ILog log)
        {
            _context = context;
            _log = log;
#if DEBUG
            _commit = false;
#endif
        }

        public ILog Log
        {
            get { return _log; }
        }

        public OperationContext Context
        {
            get { return _context; }
        }

        public HttpResponseMessage Invoke(Func<HttpResponseMessage> func)
        {
            HttpResponseMessage response;
            if (_commit)
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