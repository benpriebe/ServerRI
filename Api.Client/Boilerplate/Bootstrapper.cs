using System;
using System.Reflection;
using Api.Common.Config;

namespace Api.Client.Boilerplate
{
    public static class Bootstrapper
    {
        public static void Bootstrap(Uri baseUri, Assembly clientAssembly, Assembly middlewareAssembly, Action<Endpoint> endpointRegister)
        {
            var endpointResult = EndpointDiscovery.Discover(baseUri, clientAssembly, middlewareAssembly);

            if (endpointResult.Failure)
            {
                throw new Exception(string.Format("Unable to discover compatible endpoint at Uri '{0}'. Reasons:\n\t{1}\n", baseUri, endpointResult.ToMessageText("\n\t")));
            }
            endpointRegister(endpointResult.Value);
        }
    }
}
