using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SingleAgent.Net
{
    public static class Utils
    {
        //http://logger.io/ip
        public static string PublicIPAddress()
        {
            string uri = $"http://checkip.dyndns.org/";
            string ip = string.Empty;

            using (var client = new HttpClient())
            {
                var result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

                ip = result.Split(':')[1].Split('<')[0];
            }

            return ip.Trim();
        }

        public static bool IsLocalIpAddress(string host)
        {
            // get host IP addresses
            var hostIPs = Dns.GetHostAddresses(host);
            // get local IP addresses
            var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // test if any host IP equals to any local IP or to localhost
            foreach (var hostIp in hostIPs)
            {
                // is localhost
                if (IPAddress.IsLoopback(hostIp))
                {
                    return true;
                }
                // is local address
                if (localIPs.Contains(hostIp))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
