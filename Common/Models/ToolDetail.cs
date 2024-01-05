using Common.Models.Converters;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Common.Models
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Group: {ToolGroup}");
            sb.AppendLine($"RuntimeIdentifier: {RuntimeIdentifier}");
            sb.AppendLine($"DownloadUrl: {DownloadUrl}");
            sb.AppendLine($"DownloadFileName: {DownloadFileName}");
            sb.AppendLine($"VersionDetectionInstruction: {VersionDetectionInstruction}");
            sb.AppendLine($"InstallInstruction: {InstallInstruction}");
            sb.AppendLine($"Description: {Description}");
            sb.AppendLine($"IsActive: {IsActive}");
            sb.AppendLine($"UpdatedOn: {UpdatedOn}");
            return sb.ToString();
        }
    }
}