#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Api.Common;
using Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#endregion

namespace Api.Client
{
    public class ClientProxy
    {
        private readonly Action<HttpRequestMessage> _globalRequestMessageHandler;
        private readonly Lazy<MediaTypeFormatter> _jsonFormatter;
        private string _baseApiUri;

        public ClientProxy(string baseApiUri, Action<HttpRequestMessage> globalRequestMessageHandler = null)
        {
            _globalRequestMessageHandler = globalRequestMessageHandler;
            BaseApiUri = baseApiUri;

            _jsonFormatter = new Lazy<MediaTypeFormatter>(
                () =>
                {
                    var serializerSettings = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects, // note: resolves cyclic dependencies when serializing.
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        TypeNameHandling = TypeNameHandling.Auto,
                    };

                    serializerSettings.Converters.Add(new StringEnumConverter());

                    return new JsonMediaTypeFormatter
                    {
                        SerializerSettings = serializerSettings
                    };
                });
        }

        public string BaseApiUri
        {
            get { return _baseApiUri; }
            private set
            {
                _baseApiUri = value; 
                if (!value.EndsWith("/"))
                    _baseApiUri += "/";
            }
        }

        protected MediaTypeFormatter JsonFormatter
        {
            get { return _jsonFormatter.Value; }
        }
        
        public Task<Result<T>> Get<T>(string relativeUri, IEnumerable<MediaTypeFormatter> formatters = null)
        {
            Func<HttpResponseMessage, Result<T>> responseHandler = response =>
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {
                            var result = response.Content.ReadAsAsync<T>().Result;
                            return Result<T>.Create(result);
                        }
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        {
                            var result = response.Content.ReadAsAsync<List<Message>>(formatters ?? CreateDefaultFormatters()).Result;
                            return Result<T>.Create(result);
                        }
                    default:
                    {
                        return Result<T>.Create(GetUnexpectedResponseStatusCodeMessage(response));
                    }
                }
            };
            Func<Message, Result<T>> errorHandler = Result<T>.Create;
            return Request(relativeUri, HttpMethod.Get, responseHandler, errorHandler);
        }

        public Task<Result<T>> Get<T>(object id, IEnumerable<MediaTypeFormatter> formatters = null)
        {
            return Get<T>(id.ToString(), formatters);
        }

        public Task<Result<List<T>>> GetList<T>(IEnumerable<MediaTypeFormatter> formatters = null)
        {
            return Get<List<T>>(String.Empty);
        }

        public Task<Result<List<T>>> GetList<T>(string relativeUri, IEnumerable<MediaTypeFormatter> formatters = null)
        {
            return Get<List<T>>(relativeUri);
        }

        public Task<Result<Uri>> PostWithUri<TRequestData>(TRequestData requestData)
        {
            return PostWithUri(String.Empty, requestData);
        }
        
        public Task<Result<Uri>> PostWithUri<TRequestData>(string relativeUri, TRequestData requestData)
        {
            var returnVal = Post<TRequestData, object>(relativeUri, requestData)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    if (result.Failure)
                    {
                        return Result<Uri>.Create(result.Messages);
                    }
                    return Result<Uri>.Create(result.Value.Item1); // the Uri

                }, TaskContinuationOptions.ExecuteSynchronously);

            return returnVal;
        }

        public Task<Result<TResult>> PostWithResult<TRequestData, TResult>(TRequestData requestData)
        {
            return PostWithResult<TRequestData, TResult>(String.Empty, requestData);
        }

        public Task<Result<TResult>> PostWithResult<TRequestData, TResult>(string relativeUri, TRequestData requestData)
        {
            var returnVal = Post<TRequestData, TResult>(relativeUri, requestData)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    if (result.Failure)
                    {
                        return Result<TResult>.Create(result.Messages);
                    }
                    return Result<TResult>.Create(result.Value.Item2); 

                }, TaskContinuationOptions.ExecuteSynchronously);

            return returnVal;
        }

        public Task<Result<Tuple<Uri, TResult>>> Post<TRequestData, TResult>(TRequestData requestData)
        {
            return Post<TRequestData, TResult>(String.Empty, requestData);
        }

        public Task<Result<Tuple<Uri, TResult>>> Post<TRequestData, TResult>(string relativeUri, TRequestData requestData)
        {
            Action<HttpRequestMessage> requestHandler = request => request.Content = new ObjectContent<TRequestData>(requestData, JsonFormatter);
            Func<HttpResponseMessage, Result<Tuple<Uri, TResult>>> responseHandler = response =>
            {

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
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                    {
                        var result = response.Content.ReadAsAsync<List<Message>>().Result;
                        return Result<Tuple<Uri, TResult>>.Create(result);
                    }
                    default:
                    {
                        return Result<Tuple<Uri, TResult>>.Create(GetUnexpectedResponseStatusCodeMessage(response));
                    }
                }
            };
            
            Func<Message, Result<Tuple<Uri, TResult>>> errorHandler = Result<Tuple<Uri, TResult>>.Create;
            
            return Request(relativeUri, HttpMethod.Post, responseHandler, errorHandler, requestHandler);
        }

        public Task<Result> Put<TRequestData>(string relativeUri, TRequestData requestData)
        {
            Action<HttpRequestMessage> requestHandler = request => request.Content = new ObjectContent<TRequestData>(requestData, JsonFormatter);
            Func<HttpResponseMessage, Result> responseHandler = response =>
            {
                 switch (response.StatusCode)
                            {
                                case HttpStatusCode.NoContent:
                                case HttpStatusCode.OK:
                                    {
                                        return Result.CreateEmpty();
                                    }
                                 case HttpStatusCode.NotFound:           
                                 case HttpStatusCode.BadRequest:
                                    {
                                        var result = response.Content.ReadAsAsync<List<Message>>().Result;
                                        return Result.Create(result);
                                    }
                                default:
                                    {
                                        return Result.Create(GetUnexpectedResponseStatusCodeMessage(response));
                                    }
                            }
            };
            Func<Message, Result> errorHandler = Result.Create;
            return Request(relativeUri, HttpMethod.Put, responseHandler, errorHandler, requestHandler);
        }

        public Task<Result> Put<TRequestData>(object id, TRequestData requestData)
        {
            return Put(id.ToString(), requestData);
        }

        public Task<Result<TResult>> Put<TRequestData, TResult>(string relativeUri, TRequestData requestData)
        {
            Action<HttpRequestMessage> requestHandler = request => request.Content = new ObjectContent<TRequestData>(requestData, JsonFormatter);
            Func<HttpResponseMessage, Result<TResult>> responseHandler = response =>
            {

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        {
                            return Result<TResult>.CreateEmpty();
                        }
                    case HttpStatusCode.OK:
                        {
                            var result = response.Content.ReadAsAsync<TResult>().Result;
                            return Result<TResult>.Create(result);
                        }
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        {
                            var result = response.Content.ReadAsAsync<List<Message>>().Result;
                            return Result<TResult>.Create(result);
                        }
                    default:
                        {
                            return Result<TResult>.Create(GetUnexpectedResponseStatusCodeMessage(response));
                        }
                }
            };

            Func<Message, Result<TResult>> errorHandler = Result<TResult>.Create;

            return Request(relativeUri, HttpMethod.Post, responseHandler, errorHandler, requestHandler);
        }

        public Task<Result<TResult>> Put<TRequestData, TResult>(object id, TRequestData requestData)
        {
            return Put<TRequestData, TResult>(id.ToString(), requestData);
        }

        public Task<Result> Delete(string relativeUri)
        {
            Func<HttpResponseMessage, Result> responseHandler = response =>
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.OK:
                        {
                            return Result.CreateEmpty();
                        }
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        {
                            var result = response.Content.ReadAsAsync<List<Message>>().Result;
                            return Result.Create(result);
                        }
                    default:
                        {
                            return Result.Create(GetUnexpectedResponseStatusCodeMessage(response));
                        }
                }
            };
            Func<Message, Result> errorHandler = Result.Create;
            return Request(relativeUri, HttpMethod.Delete, responseHandler, errorHandler);
        }

        public Task<Result> Delete(object id)
        {
            return Delete(id.ToString());
        }

        private Task<TResult> Request<TResult>(string relativeUri, HttpMethod httpMethod, Func<HttpResponseMessage, TResult> responseMessageHandler, Func<Message, TResult> errorHandler, Action<HttpRequestMessage> requestMessageHandler = null)
        {
            try
            {
                var requestUri = new Uri(BaseApiUri + relativeUri);
                var request = new HttpRequestMessage(httpMethod, requestUri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (_globalRequestMessageHandler != null)
                    _globalRequestMessageHandler(request);

                if (requestMessageHandler != null)
                    requestMessageHandler(request);

                var httpClient = HttpClientFactory.Create(new CompressionHandler());
                var resultTask = httpClient.SendAsync(request).ContinueWith(
                    sendTask =>
                    {
                        httpClient.Dispose();
                        sendTask.PropagateParentTaskStatus(requestUri.AbsolutePath);
                        return responseMessageHandler(sendTask.Result);

                    }, TaskContinuationOptions.ExecuteSynchronously);

                return resultTask;
            }
            catch (Exception e)
            {
                var message = new Message(MessageLevel.Error, (int)MessageCodes.UnexpectedApiRequestError, String.Format("Unexpected error accessing -> {0} {1} - {2}", httpMethod.Method, BaseApiUri + relativeUri, e.Message), String.Empty);
                var task = new Task<TResult>(() => errorHandler(message));
                task.Start();
                return task;
            }

        }
        
        #region Helper Methods/Classes 

        protected virtual IEnumerable<MediaTypeFormatter> CreateDefaultFormatters()
        {
            return new []
            {
                JsonFormatter,
                new XmlMediaTypeFormatter(),
                new FormUrlEncodedMediaTypeFormatter()
            };
        }

        private static Message GetUnexpectedResponseStatusCodeMessage(HttpResponseMessage response)
        {
            var message = new Message(MessageLevel.Error, (int)MessageCodes.UnexpectedResponseCode, String.Format("Unexpected response accessing -> {0} {1} - {2}: ", response.RequestMessage.Method.Method, response.RequestMessage.RequestUri.AbsoluteUri, response.StatusCode), string.Empty);
            return message;
        }


        public class DecompressedContent : HttpContent
        {
            private readonly string _encodingType;
            private readonly HttpContent _originalContent;


            public DecompressedContent(HttpContent originalContent, string encodingType)
            {
                _originalContent = originalContent;
                _encodingType = encodingType;
            }


            protected override bool TryComputeLength(out long length)
            {
                length = -1;

                return false;
            }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                var originalStream = await _originalContent.ReadAsStreamAsync();
                Stream decompressedStream = null;

                if (_encodingType == "gzip")
                {
                    decompressedStream = new GZipStream(originalStream, CompressionMode.Decompress, leaveOpen: true);
                }
                else if (_encodingType == "deflate")
                {
                    decompressedStream = new DeflateStream(originalStream, CompressionMode.Decompress, leaveOpen: true);
                }

                if (decompressedStream != null)
                {
                    await decompressedStream.CopyToAsync(stream).ContinueWith(tsk =>
                    {
                        if (decompressedStream != null)
                        {
                            decompressedStream.Dispose();
                        }
                    });
                }
            }
        }

        protected class CompressionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                var response = await base.SendAsync(request, cancellationToken);
                var streamContent = response.Content as StreamContent;
                if (streamContent != null)
                {
                    if (streamContent.Headers.ContentEncoding.Contains("gzip"))
                    {
                        response.Content = WrapWithDecompressedContent(streamContent, "gzip");
                    }
                    else if (streamContent.Headers.ContentEncoding.Contains("deflate"))
                    {
                        response.Content = WrapWithDecompressedContent(streamContent, "deflate");
                    }
                }

                return response;
            }


            protected HttpContent WrapWithDecompressedContent(HttpContent streamContent, string encodingScheme)
            {
                var replacedContent = new DecompressedContent(streamContent, encodingScheme);
                foreach (var header in streamContent.Headers)
                {
                    replacedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                return replacedContent;
            }
        }

        #endregion Helper Methods/Classes
    }
}
