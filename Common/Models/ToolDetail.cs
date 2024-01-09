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

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("downloadFileName")]
        public string DownloadFileName { get; set; }

        [JsonProperty("installCheckInstruction")]
        public InstallCheckInstruction InstallCheckInstruction { get; set; }

        [JsonProperty("installInstruction")]
        public InstallInstruction InstallInstruction { get; set; }

        [JsonProperty("upgradeInstruction")]
        public UpgradeInstruction UpgradeInstruction { get; set; }

        [JsonProperty("downgradeInstruction")]
        public DowngradeInstruction DowngradeInstruction { get; set; }

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
            sb.AppendLine($"InstallCheckInstruction: {InstallCheckInstruction}");
            sb.AppendLine($"InstallInstruction: {InstallInstruction}");
            sb.AppendLine($"Description: {Description}");
            sb.AppendLine($"IsActive: {IsActive}");
            sb.AppendLine($"UpdatedOn: {UpdatedOn}");
            return sb.ToString();
        }
    }
}