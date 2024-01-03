using Newtonsoft.Json;
using ToolManager.Converters;

namespace ToolManager.Models
{
    public class InstallIStatusDetection
    {
        [JsonConverter(typeof(VersionDetectionTypeConverter))]
        [JsonProperty("type")] public VersionDetectionType Type { get; set; }

        [JsonProperty("key")] public string Key { get; set; }

        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("minVersion")] public string MinVersion { get; set; }

        [JsonProperty("maxVersion")] public string MaxVersion { get; set; }
    }
}