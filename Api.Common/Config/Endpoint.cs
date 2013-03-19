using System;
using System.Collections.Generic;
using System.Reflection;
using Api.Common.Extensions;

namespace Api.Common.Config
{
    public class Endpoint
    {
        public Endpoint()
        {
        }

        public Endpoint(string id, Assembly serverAssembly, Assembly middlewareAssembly)
        {
            Links = new Dictionary<string, Link>();
            ServerVersion = serverAssembly.ToVersion();
            MiddlewareVersion = middlewareAssembly.ToVersion();
            Id = id;
        }

        public string Id { get; set; }
        public string ServerVersion { get; set; }
        public string MiddlewareVersion { get; set; }
        public Dictionary<string, Link> Links { get; set; }

        /// <summary>
        /// This value is set on the client-side rather than on the server side as the server
        /// side doesn't really care about the base URI. Clients can set this so that all
        /// of the proxies in the client application can easily get access to the full URL.
        /// </summary>
        public Uri BaseUri { get; private set; }

        public Link Get(string id)
        {
            var result = default(Link);
            Links.TryGetValue(id, out result);
            return result;
        }

        public Endpoint Add(string id, string path, params string[] methods)
        {
            var link = new Link
            {
                Id = id,
                Path = path
            };

            link.Methods.AddRange(methods);

            Links.Add(id, link);

            return this;
        }

        public void SetBaseUri(string baseUri)
        {
            SetBaseUri(new Uri(baseUri));
        }

        public void SetBaseUri(Uri baseUri)
        {
            BaseUri = baseUri;
            foreach (var link in Links.Values)
            {
                link.SetBaseUri(BaseUri);
            }
        }
    }
}
