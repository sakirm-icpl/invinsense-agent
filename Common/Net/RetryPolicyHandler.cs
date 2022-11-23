using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Net
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private readonly string _key;

        public RetryPolicyHandler(string key)
        {
            _key = key;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                try
                {
                    // base.SendAsync calls the inner handler
                    var response = await base.SendAsync(request, cancellationToken);

                    watch.Stop();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //Add log here
                    }

                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        // 503 Service Unavailable
                        // Wait a bit and try again later
                        await Task.Delay(5000, cancellationToken);
                        continue;
                    }

                    if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                    {
                        // 429 Too many requests
                        // Wait a bit and try again later
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }

                    return response;
                }
                catch (Exception ex) when (IsNetworkError(ex))
                {
                    // Network error
                    // Wait a bit and try again later
                    await Task.Delay(2000, cancellationToken);
                }
            }
        }

        private static bool IsNetworkError(Exception ex)
        {
            // Check if it's a network error
            if (ex.GetType() == typeof(SocketException) || ex.GetType() == typeof(IOException))
            {
                return true;
            }

            if (ex.InnerException != null)
            {
                return IsNetworkError(ex.InnerException);
            }

            return false;
        }
    }
}