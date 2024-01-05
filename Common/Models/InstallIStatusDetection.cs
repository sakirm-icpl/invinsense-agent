using Common.Models.Converters;
using Newtonsoft.Json;
using System.Text;

namespace Common.Models
{
    public class InstallIStatusDetection
    {
        [JsonConverter(typeof(VersionDetectionTypeConverter))]
        [JsonProperty("type")] public VersionDetectionType Type { get; set; }

        [JsonProperty("key")] public string Key { get; set; }

        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("minVersion")] public string MinVersion { get; set; }

        [JsonProperty("maxVersion")] public string MaxVersion { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Key: {Key}");
            sb.AppendLine($"Version: {Version}");
            sb.AppendLine($"MinVersion: {MinVersion}");
            sb.AppendLine($"MaxVersion: {MaxVersion}");
            return sb.ToString();
        }
    }
}