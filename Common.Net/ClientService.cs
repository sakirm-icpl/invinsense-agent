using Common.ConfigProvider;
using Common.Models;
using Common.Utils;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Net
{
    public class ClientService
    {
        protected readonly HttpClientConfig Configuration;

        private readonly ILogger logger = Log.ForContext<ClientService>();

        public ClientService(string serviceName)
        {
            Configuration = FileConfigProvider.Load<HttpClientConfig>(serviceName);
        }

        public ClientService(HttpClientConfig config)
        {
            Configuration = config;
        }

        private HttpClient InitClient()
        {
            var pipeline = new RetryPolicyHandler(Configuration.Name);

            if (Configuration.TimeOut == 0)
            {
                Configuration.TimeOut = 5;
                logger.Verbose("Time out not found in {Name}", Configuration.Name);
            }

            var client = new HttpClient(pipeline, disposeHandler: false)
            {
                Timeout = TimeSpan.FromSeconds(Configuration.TimeOut)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            AppendHeaders(client.DefaultRequestHeaders);

            return client;
        }

        /// <summary>
        /// Append base headers from configuration
        /// </summary>
        /// <param name="headers"></param>
        public virtual void AppendHeaders(HttpRequestHeaders headers)
        {
            //Override this method to append custom headers.
            foreach (var item in Configuration.BaseHeaders)
            {
                headers.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Before Send Request
        /// </summary>
        /// <param name="headers"></param>
        public virtual void BeforeRequestSent(HttpRequestMessage request, string payload)
        {

        }

        public async Task<ProcessResult<T, E>> InvokeAsync<T, E>(HttpMethod methodType, string path, object postData = null) where T : IModel where E : IException
        {
            var result = await InvokeAsync(methodType, path, postData);

            var response = Encoding.UTF8.GetString(result.Response, 0, result.Response.Length);

            try
            {
                if (result.IsSuccess)
                {
                    return ProcessResult<T, E>.Success(result.Response.Length > 0 ? JsonConvert.DeserializeObject<T>(response, SerializationExtension.DefaultOptions) : default);
                }
                else
                {
                    return ProcessResult<T, E>.Fail(result.Response.Length > 0 ? JsonConvert.DeserializeObject<E>(response, SerializationExtension.DefaultOptions) : default);
                }
            }
            catch (Exception ex)
            {
                logger.Error("HTTP:{IsSuccess} - {Message} : payload: {Payload}", result.IsSuccess, ex.Message, response);
                return ProcessResult<T, E>.Fail("Response", ex.Message);
            }
        }

        public async Task<ProcessResult<T>> InvokeAsync<T>(HttpMethod methodType, string path, object postData = null) where T : IModel
        {
            var result = await InvokeAsync(methodType, path, postData);

            var response = Encoding.UTF8.GetString(result.Response, 0, result.Response.Length);

            try
            {
                if (result.IsSuccess)
                {
                    if (result.Response.Length > 0)
                    {
                        return ProcessResult<T>.Success(JsonConvert.DeserializeObject<T>(response, SerializationExtension.DefaultOptions));
                    }
                    else
                    {
                        return ProcessResult<T>.Success(default);
                    }
                }
                else
                {
                    if (result.Response.Length > 0)
                    {
                        return ProcessResult<T>.Fail(JsonConvert.DeserializeObject<Error>(response, SerializationExtension.DefaultOptions));
                    }
                    else
                    {
                        return ProcessResult<T>.Fail(result.StatusCode.ToString(), "Error processing request");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HTTP:{IsSuccess} - {Message} : payload: {Payload}", result.IsSuccess, ex.Message, response);
                return ProcessResult<T>.Fail("Response", ex.Message);
            }
        }

        public async Task<ApiResponse> InvokeAsync(HttpMethod methodType, string path, object postData = null)
        {
            var client = InitClient();

            try
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();

                var request = new HttpRequestMessage
                {
                    Method = methodType,
                    RequestUri = new Uri($"{Configuration.BaseUrl}{path}")
                };

                var payload = string.Empty;

                if ((methodType == HttpMethod.Post || methodType == HttpMethod.Put) && postData != null)
                {
                    payload = JsonConvert.SerializeObject(postData);
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                }

                if (logger.IsEnabled(LogEventLevel.Verbose))
                {
                    logger.Verbose("REQUEST: Method: {MethodType}, URI: {BaseUrl}{Path}, Payload: {Payload}", methodType, Configuration.BaseUrl, path, payload);
                }

                BeforeRequestSent(request, payload);

                var httpResponseMessage = await client.SendAsync(request);
                var resultBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();

                watch.Stop();

                if (logger.IsEnabled(LogEventLevel.Verbose))
                {
                    logger.Verbose("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }
                else if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.Error("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }
                else if (watch.ElapsedMilliseconds > 5000)
                {
                    logger.Warning("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }

                return new ApiResponse
                {
                    Response = resultBytes,
                    StatusCode = httpResponseMessage.StatusCode
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(HttpRequestException) ||
                    ex.GetType() == typeof(TimeoutException) ||
                    ex.GetType() == typeof(TaskCanceledException))
                {
                    logger.Error("Error in Invoke Method:{path}-ResponseCode:{Message}", path, ex.Message);

                    return new ApiResponse
                    {
                        Response = Array.Empty<byte>()
                    };
                }

                logger.Error("Error Type: {Type} in Invoke Method:{path}-ResponseCode:{Message}", ex.GetType(), path, ex.Message);

                throw;
            }
        }
    }
}
