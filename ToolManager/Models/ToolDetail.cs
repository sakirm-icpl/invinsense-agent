using Common.Models;
using Newtonsoft.Json;
using System;
using ToolManager.Converters;

namespace ToolManager.Models
{
    public class ToolDetail : IModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(ToolGroupConverter))]
        [JsonProperty("group")]
        public ToolGroup ToolGroup { get; set; }        

        [JsonProperty("runtimeIdentifier")]
        public string RuntimeIdentifier { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("downloadFileName")]
        public string DownloadFileName { get; set; }

        [JsonProperty("versionDetectionInstruction")]
        public InstallIStatusDetection VersionDetectionInstruction { get; set; }

        [JsonProperty("installInstruction")]
        public InstallInstruction InstallInstruction { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("updatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}