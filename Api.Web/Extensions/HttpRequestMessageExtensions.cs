#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Api.Common;

#endregion


namespace Api.Web.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        private static HttpResponseMessage CreateDefaultResponseFor(this HttpRequestMessage request, Result result)
        {
            return result.Success
                ? request.CreateResponse(HttpStatusCode.NoContent)
                : request.CreateFailureResponseFor(result.Messages, result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);
        }

        private static HttpResponseMessage CreateDefaultResponseFor<TResult, TValue>(this HttpRequestMessage request, Result<TResult> result, TValue value)
        {
            if (result.Success)
            {
                return value != null
                    ? request.CreateResponse(HttpStatusCode.OK, value)
                    : request.CreateResponse(HttpStatusCode.NoContent);
            }

            return request.CreateFailureResponseFor(result.Messages, result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);
        }

        public static HttpResponseMessage CreateFailureResponseFor(this HttpRequestMessage request, IList<Message> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return messages != null
                ? request.CreateResponse(statusCode, messages)
                : request.CreateResponse(statusCode);
        }

        public static HttpResponseMessage CreateGetResponseFor(this HttpRequestMessage request, Result result)
        {
            return request.CreateDefaultResponseFor(result);
        }

        public static HttpResponseMessage CreateGetResponseFor<T>(this HttpRequestMessage request, Result<T> result)
        {
            return request.CreateGetResponseFor(result, result.Value);
        }

        public static HttpResponseMessage CreateGetResponseFor<T, TSource, TResult>(this HttpRequestMessage request, Result<T> result, Func<TSource, TResult> selector)
            where T : IEnumerable<TSource>
        {
            return request.CreateGetResponseFor(result, result.Value != null ? result.Value.Select(selector) : null);
        }

        private static HttpResponseMessage CreateGetResponseFor<TResult, TValue>(this HttpRequestMessage request, Result<TResult> result, TValue value)
        {
            if (result.Success)
            {
                return value != null
                    ? request.CreateResponse(HttpStatusCode.OK, value)
                    : request.CreateResponse(HttpStatusCode.NoContent);
            }

            return request.CreateFailureResponseFor(result.Messages, HttpStatusCode.NotFound);
        }

        public static HttpResponseMessage CreatePostResponseFor(this HttpRequestMessage request, Result result, string locationRouteName, object locationRouteValues)
        {
            return request.CreatePostResponseFor(result, () => request.GetAbsoluteRouteUriFor(locationRouteName, locationRouteValues));
        }

        public static HttpResponseMessage CreatePostResponseFor(this HttpRequestMessage request, Result result, Uri location)
        {
            return request.CreatePostResponseFor(result, () => location);
        }

        public static HttpResponseMessage CreatePostResponseFor(this HttpRequestMessage request, Result result, Func<Uri> locationProvider)
        {
            if (result.Success)
            {
                var response = request.CreateResponse(HttpStatusCode.Created);
                response.Headers.Location = locationProvider.Invoke();
                return response;
            }

            return request.CreateFailureResponseFor(result.Messages, result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);
        }

        public static HttpResponseMessage CreatePostResponseFor<T>(this HttpRequestMessage request, Result<T> result, string locationRouteName, object locationRouteValues)
        {
            return request.CreatePostResponseFor(result, result.Value, () => request.GetAbsoluteRouteUriFor(locationRouteName, locationRouteValues));
        }

        public static HttpResponseMessage CreatePostResponseFor<T>(this HttpRequestMessage request, Result<T> result, Uri location)
        {
            return request.CreatePostResponseFor(result, result.Value, () => location);
        }

        public static HttpResponseMessage CreatePostResponseFor<T, TSource, TResult>(this HttpRequestMessage request, Result<T> result, string locationRouteName, object locationRouteValues, Func<TSource, TResult> selector)
            where T : IEnumerable<TSource>
        {
            return request.CreatePostResponseFor(result, GetValueFor(result, selector), () => request.GetAbsoluteRouteUriFor(locationRouteName, locationRouteValues));
        }

        public static HttpResponseMessage CreatePostResponseFor<T, TSource, TResult>(this HttpRequestMessage request, Result<T> result, Uri location, Func<TSource, TResult> selector)
            where T : IEnumerable<TSource>
        {
            return request.CreatePostResponseFor(result, GetValueFor(result, selector), () => location);
        }

        private static IEnumerable<TResult> GetValueFor<T, TSource, TResult>(Result<T> result, Func<TSource, TResult> selector) where T : IEnumerable<TSource>
        {
            return result.Value != null ? result.Value.Select(selector) : null;
        }

        private static HttpResponseMessage CreatePostResponseFor<TResult, TValue>(this HttpRequestMessage request, Result<TResult> result, TValue value, Func<Uri> locationProvider)
        {
            if (result.Success)
            {
                var response = value != null
                    ? request.CreateResponse(HttpStatusCode.Created, value)
                    : request.CreateResponse(HttpStatusCode.Created);
                response.Headers.Location = locationProvider.Invoke();
                return response;
            }

            return request.CreateFailureResponseFor(result.Messages, result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);
        }

        public static HttpResponseMessage CreatePutResponseFor(this HttpRequestMessage request, Result result)
        {
            return request.CreateDefaultResponseFor(result);
        }

        public static HttpResponseMessage CreatePutResponseFor<T>(this HttpRequestMessage request, Result<T> result)
        {
            return request.CreateDefaultResponseFor(result, result.Value);
        }

        public static HttpResponseMessage CreatePutResponseFor<T, TSource, TResult>(this HttpRequestMessage request, Result<T> result, Func<TSource, TResult> selector) where T : IEnumerable<TSource>
        {
            return request.CreateDefaultResponseFor(result, result.Value != null ? result.Value.Select(selector) : null);
        }

        public static HttpResponseMessage CreateDeleteResponseFor(this HttpRequestMessage request, Result result)
        {
            return request.CreateDefaultResponseFor(result);
        }

        public static HttpResponseMessage CreateDeleteResponseFor<T>(this HttpRequestMessage request, Result<T> result)
        {
            return request.CreateDefaultResponseFor(result, result.Value);
        }

        public static HttpResponseMessage CreateDeleteResponseFor<T, TSource, TResult>(this HttpRequestMessage request, Result<T> result, Func<TSource, TResult> selector) where T : IEnumerable<TSource>
        {
            return request.CreateDefaultResponseFor(result, result.Value != null ? result.Value.Select(selector) : null);
        }

        private static Uri GetAbsoluteRouteUriFor(this HttpRequestMessage request, string routeName, object routeValues)
        {
            return new Uri(request.RequestUri, request.GetUrlHelper().Route(routeName, routeValues));
        }


        #region Generice Response Convenience Methods

        /* Generic Response Convenience Methods - only use if you don't have a Result object */

        public static HttpResponseMessage CreateBadRequestResponse(this HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public static HttpResponseMessage CreateNotFoundResponse(this HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.NotFound);
        }

        public static HttpResponseMessage CreateNotImplementedResponse(this HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        public static HttpResponseMessage CreateForbiddenResponse(this HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}