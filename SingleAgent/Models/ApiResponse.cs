using System.Net;

namespace SingleAgent.Models
{
    public class ApiResponse
    {
        public byte[] Response { get; set; }

        public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 400;

        public HttpStatusCode StatusCode { get; set; }
    }
}
