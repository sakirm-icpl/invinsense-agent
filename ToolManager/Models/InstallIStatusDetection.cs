using Newtonsoft.Json;

namespace ToolManager.Models
{
    public class InstallIStatusDetection
    {
        [JsonProperty("type")] public VersionDetectionType Type { get; set; }

        [JsonProperty("key")] public string Key { get; set; }

        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("minVersion")] public string MinVersion { get; set; }

        [JsonProperty("maxVersion")] public string MaxVersion { get; set; }
    }
}