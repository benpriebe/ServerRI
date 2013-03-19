using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Api.Common;
using Api.Common.Config;
using Api.Common.Extensions;

namespace Api.Client
{
    public static class EndpointDiscovery
    {
        public static Result<Endpoint> Discover(Uri baseUri, Assembly clientAssembly, Assembly middlewareAssembly)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(baseUri).Result;
                    var result = response.Content.ReadAsAsync<Endpoint>().Result;

                    var errors = new List<Message>();
                    if (!clientAssembly.IsVersionCompatible(result.ServerVersion))
                    {
                        var message = new Message(MessageLevel.Error, 403, string.Format("Client v{0} is not compatible with Server v{1}", clientAssembly.ToVersion(), result.ServerVersion), string.Empty);
                        errors.Add(message);
                    }

                    if (!middlewareAssembly.IsVersionCompatible(result.MiddlewareVersion))
                    {
                        var message = new Message(MessageLevel.Error, 404, string.Format("Client Middleware v{0} is not compatible with Server Middleware v{1}", clientAssembly.ToVersion(), result.ServerVersion), string.Empty);
                        errors.Add(message);
                    }

                    if (errors.Count > 0)
                    {
                        return Result<Endpoint>.Create(errors);
                    }

                    result.SetBaseUri(baseUri);
                    return Result<Endpoint>.Create(result);
                }
            }
            catch (Exception)
            {
                var message = new Message(MessageLevel.Error, 402, "Unexpected error accessing Endpoint.", string.Empty);
                return Result<Endpoint>.Create(message);
            }
        }
    }
}
