using System;
using System.Collections.Generic;

namespace Api.Common.Config
{
    public class Link
    {
        public Link()
        {
            Methods = new List<string>();
        }

        /// <summary>
        /// HTTP method, should be one of: <list type="string">
        /// <item>GET</item>
        /// <item>PUT</item>
        /// <item>POST</item>
        /// <item>DELETE</item>
        /// </list>
        /// </summary>
        public List<string> Methods { get; set; }

        public string Path { get; set; }

        public string Id { get; set; }

        public Uri Uri { get; private set; }

        public void SetBaseUri(Uri baseUri)
        {
            Uri = new Uri(baseUri, Path);
        }
    }
}
