using System;
using System.Web;

namespace Api.Client.Extensions
{
    public static class UriExtensions
    {
        public static Uri AddQueryParam(this Uri uri, string key, object value)
        {
            var builder = new UriBuilder(uri);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            queryParams.Add(key, value.ToString());
            builder.Query = queryParams.ToString();
            return builder.Uri;
        }
    }
}