using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Api.Common;
using Api.Common.Config;

namespace Api.Client
{
    public abstract class ClientProxy
    {
        protected readonly Link Link;

        protected ClientProxy(Link link)
        {
            Link = link;
        }

        protected ClientProxy(Endpoint endpoint, string linkId, params string[] methods)
        {
            Link = endpoint.Get(linkId);

            if (Link == null)
            {
                throw new ArgumentException(string.Format("Endpoint doesn't contain expected link '{0}'.", linkId));
            }

            foreach (var method in methods)
            {
                if (!Link.Methods.Contains(method))
                {
                    throw new ArgumentException(string.Format("Endpoint link '{0}' does not support expected method '{1}'.", linkId, method));
                }
            }
        }

        protected Result<List<T>> GetList<T>(Func<Uri, Uri> uriCreator = null)
        {
            uriCreator = uriCreator ?? new Func<Uri, Uri>(x => x);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uriCreator(Link.Uri)).Result;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            {
                                var result = response.Content.ReadAsAsync<List<T>>().Result;
                                return Result<List<T>>.Create(result);
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result<List<T>>.Create(result);
                            }
                        default:
                            {
                                var message = new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + response.StatusCode, string.Empty);
                                return Result<List<T>>.Create(message);
                            }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: should we not put some detail of the exception in here?
                var message = new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
                return Result<List<T>>.Create(message);
            }
        }

        protected Result<T> Get<T>(Func<Uri, Uri> uriCreator = null)
        {
            return Get<T>(string.Empty, uriCreator);
        }

        protected Result<T> Get<T>(string id, Func<Uri, Uri> uriCreator = null)
        {
            uriCreator = uriCreator ?? new Func<Uri, Uri>(x => x);

            try
            {
                using (var client = new HttpClient())
                {
                    var initialUri = string.IsNullOrWhiteSpace(id) ? Link.Uri : new Uri(Link.Uri, id);
                    var response = client.GetAsync(uriCreator(initialUri)).Result;

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
                                var message = new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + response.StatusCode, string.Empty);
                                return Result<T>.Create(message);
                            }
                    }
                }
            }
            catch (Exception)
            {
                var message = new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
                return Result<T>.Create(message);
            }
        }

        protected Result<Uri> Post<T>(T body, Func<Uri, Uri> uriCreator = null)
        {
            var result = Post<T, object>(body, uriCreator);
            if (result.Failure)
            {
                return Result<Uri>.Create(result.Messages);
            }
            return Result<Uri>.Create(result.Value.Item1);
        }

        protected Result<Tuple<Uri, TResult>> Post<TContent, TResult>(TContent body, Func<Uri, Uri> uriCreator = null)
        {
            uriCreator = uriCreator ?? new Func<Uri, Uri>(x => x);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsJsonAsync(uriCreator(Link.Uri).ToString(), body).Result;

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
                                    return Result<Tuple<Uri, TResult>>.Create(Tuple.Create<Uri, TResult>(locationUri, result));
                                }
                                return Result<Tuple<Uri, TResult>>.Create(Tuple.Create<Uri, TResult>(locationUri, default(TResult)));
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                return Result<Tuple<Uri, TResult>>.Create(result);
                            }
                        default:
                            {
                                var message = new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + response.StatusCode, string.Empty);
                                return Result<Tuple<Uri, TResult>>.Create(message);
                            }
                    }
                }
            }
            catch (Exception)
            {
                var message = new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
                return Result<Tuple<Uri, TResult>>.Create(message);
            }
        }

        protected Result Put<T>(T body, Func<Uri, Uri> uriCreator = null)
        {
            return Put<T>(string.Empty, body, uriCreator);
        }

        protected Result Put<T>(string id, T body, Func<Uri, Uri> uriCreator = null)
        {
            uriCreator = uriCreator ?? new Func<Uri, Uri>(x => x);

            try
            {
                using (var client = new HttpClient())
                {
                    var initialUri = string.IsNullOrWhiteSpace(id) ? Link.Uri : new Uri(Link.Uri, id);
                    var response = client.PutAsJsonAsync(uriCreator(initialUri).ToString(), body).Result;

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
                                var message = new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + response.StatusCode, string.Empty);
                                return Result.Create(message);
                            }
                    }
                }
            }
            catch (Exception)
            {
                var message = new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
                return Result.Create(message);
            }
        }

        protected Result Delete(Func<Uri, Uri> uriCreator = null)
        {
            return Delete(string.Empty, uriCreator);
        }

        protected Result Delete(string id, Func<Uri, Uri> uriCreator = null)
        {
            uriCreator = uriCreator ?? new Func<Uri, Uri>(x => x);

            try
            {
                using (var client = new HttpClient())
                {
                    var initialUri = string.IsNullOrWhiteSpace(id) ? Link.Uri : new Uri(Link.Uri, id);
                    var response = client.DeleteAsync(uriCreator(initialUri)).Result;

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
                                var message = new Message(MessageLevel.Error, 402, "Unexpected response accessing API: " + response.StatusCode, string.Empty);
                                return Result.Create(message);
                            }
                    }
                }
            }
            catch (Exception)
            {
                var message = new Message(MessageLevel.Error, 401, "Unexpected error accessing API.", string.Empty);
                return Result.Create(message);
            }
        }
    }
}
