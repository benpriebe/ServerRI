#region Using directives

using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using Contracts.Data;
using Contracts.Models;
using Core;
using Core.Extensions;

#endregion


namespace Providers.External
{
    public class ExternalProvider : IExternalProvider
    {
        private readonly OperationContext _context;
        private readonly ILog _log;

        public ExternalProvider(OperationContext context, ILog log)
        {
            _context = context;
            _log = log;
        }

        public IList<Widget> GetWidgets()
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), _context)))
            {
                return new[]
                {
                    new Widget
                    {
                        Name = "Widget 1"
                    },
                    new Widget
                    {
                        Name = "Widget 2"
                    }
                };
            }
        }
    }
}