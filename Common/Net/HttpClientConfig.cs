using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Net
{
    public class HttpClientConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("authRequired")]
        public bool AuthRequired { get; set; }

        [JsonProperty("timeout")]
        public int TimeOut { get; set; }

        [JsonProperty("baseHeaders")]
        public Dictionary<string, string> BaseHeaders { get; set; }

        [JsonProperty("extraParams")]
        public Dictionary<string, string> ExtraParams { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, BaseUrl:{BaseUrl}";
        }
    }
}
