using Common.Models.Converters;
using Newtonsoft.Json;
using System.Text;

namespace Common.Models
{
    public class InstallCheckInstruction
    {
        [JsonConverter(typeof(VersionDetectionTypeConverter))]
        [JsonProperty("type")] 
        public VersionDetectionType Type { get; set; }

        [JsonProperty("key")] 
        public string Key { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Key: {Key}");
            return sb.ToString();
        }
    }
}