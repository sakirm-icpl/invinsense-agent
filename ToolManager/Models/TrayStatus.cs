using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Persistence
{
    public class TrayStatus
    {
        public TrayStatus()
        {
            ToolStatuses = new List<ToolStatus>();
        }

        [JsonProperty("errorCode")] public int ErrorCode { get; set; }

        [JsonProperty("errorMessage")] public string ErrorMessage { get; set; }

        [JsonProperty("toolStatuses")] public List<ToolStatus> ToolStatuses { get; set; }
    }

    public class VersionDetectionInstruction
    {
        [JsonProperty("type")] public VersionDetectionType Type { get; set; }

        [JsonProperty("path")] public string Path { get; set; }

        [JsonProperty("pattern")] public string Pattern { get; set; }
    }

    public enum VersionDetectionType
    {
        FilePath,
        FileInfo,
        FileContent,
        Registry
    }
}