using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Api.Common;
using Api.Common.Config;

namespace Api.Client
{
    public class Proxy
    {
        private readonly Uri _baseAddress;
        private readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter();

        public Proxy()
        {
            JsonSerializerConfig.Configure(_formatter);
        }

        public Proxy(Uri baseAddress) : this()
        {
            _baseAddress = baseAddress;
        }

        public Result<T> Get<T>(Uri uri)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = client.GetAsync(uri).Result;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            {
                                var result = response.Content.ReadAsAsync<T>().Result;
                                return Result<T>.Create(result);
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result<T>.Create(result);
                            }
                        case HttpStatusCode.NotFound:
                            {
                                return Result<T>.CreateEmpty();
                            }
                        default:
                            {
                                return Result<T>.Create(CreateUnexpectedResponseMessageFor(response.StatusCode));
                            }
                    }
                }
            }
            catch (Exception)
            {
                return Result<T>.Create(CreateUnexpectedErrorMessage());
            }
        }

        public Result<Uri> Post<T>(Uri uri, T body)
        {
            var result = Post<T, object>(uri, body);
            if (result.Failure)
            {
                return Result<Uri>.Create(result.Messages);
            }
            return Result<Uri>.Create(result.Value.Item1);
        }

        public Result<Tuple<Uri, TResult>> Post<TContent, TResult>(Uri uri, TContent body)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = client.PostAsJsonAsync(uri.ToString(), body).Result;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NoContent:
                            {
                                return Result<Tuple<Uri, TResult>>.CreateEmpty();
                            }
                        case HttpStatusCode.OK:
                            {
                                var result = response.Content.ReadAsAsync<TResult>().Result;
                                return Result<Tuple<Uri, TResult>>.Create(Tuple.Create<Uri, TResult>(null, result));
                            }
                        case HttpStatusCode.Created:
                            {
                                var locationUri = response.Headers.Location;
                                if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value > 0)
                                {
                                    var result = response.Content.ReadAsAsync<TResult>().Result;
                                    return Result<Tuple<Uri, TResult>>.Create(Tuple.Create(locationUri, result));
                                }
                                return Result<Tuple<Uri, TResult>>.Create(Tuple.Create(locationUri, default(TResult)));
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result<Tuple<Uri, TResult>>.Create(result);
                            }
                        default:
                            {
                                return Result<Tuple<Uri, TResult>>.Create(CreateUnexpectedResponseMessageFor(response.StatusCode));
                            }
                    }
                }
            }
            catch (Exception)
            {
                return Result<Tuple<Uri, TResult>>.Create(CreateUnexpectedErrorMessage());
            }
        }

        public Result Put<T>(Uri uri, T body)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = client.PutAsJsonAsync(uri.ToString(), body).Result;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.NoContent:
                            {
                                return Result.CreateEmpty();
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result.Create(result);
                            }
                        default:
                            {
                                return Result.Create(CreateUnexpectedResponseMessageFor(response.StatusCode));
                            }
                    }
                }
            }
            catch (Exception)
            {
                return Result.Create(CreateUnexpectedErrorMessage());
            }
        }

        public Result Delete(Uri uri)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = client.DeleteAsync(uri).Result;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.NoContent:
                            {
                                return Result.CreateEmpty();
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result.Create(result);
                            }
                        default:
                            {
                                return Result.Create(CreateUnexpectedResponseMessageFor(response.StatusCode));
                            }
                    }
                }
            }
            catch (Exception)
            {
                return Result.Create(CreateUnexpectedErrorMessage());
            }
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        private static Message CreateUnexpectedErrorMessage()
        {
            return new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
        }

        private static Message CreateUnexpectedResponseMessageFor(HttpStatusCode statusCode)
        {
            return new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + statusCode, string.Empty);
        }
    }
}