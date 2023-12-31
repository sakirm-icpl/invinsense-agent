using Newtonsoft.Json;
using ToolManager.Converters;
using ToolManager.Models;

namespace Common.Persistence
{
    public class ToolDetail : Models.IModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(ToolGroupConverter))]
        [JsonProperty("group")]
        public ToolGroup ToolGroup { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("minVersion")]
        public string MinVersion { get; set; }

        [JsonProperty("maxVersion")]
        public string MaxVersion { get; set; }

        [JsonProperty("runtimeIdentifier")]
        public string RuntimeIdentifier { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("destinationPath")]
        public string DestinationPath { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}