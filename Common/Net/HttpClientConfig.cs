using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common.Net
{
    public class HttpClientConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonPropertyName("authRequired")]
        public bool AuthRequired { get; set; }

        [JsonPropertyName("timeout")]
        public int TimeOut { get; set; }

        [JsonPropertyName("baseHeaders")]
        public Dictionary<string, string> BaseHeaders { get; set; }

        [JsonPropertyName("extraParams")]
        public Dictionary<string, string> ExtraParams { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, BaseUrl:{BaseUrl}";
        }
    }
}
