using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Models
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
}