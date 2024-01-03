using Newtonsoft.Json;
using System.Collections.Generic;
using ToolManager.Converters;

namespace ToolManager.Models
{
    public class InstallInstruction
    {
        [JsonConverter(typeof(InstallTypeConverter))]
        [JsonProperty("installType")]
        public InstallType InstallType { get; set; }

        [JsonProperty("installerFile")]
        public string InstallerFile { get; set; }

        [JsonProperty("installArgs")]
        public List<string> InstallArgs { get; set; }

        [JsonProperty("uninstallArgs")]
        public List<string> UninstallArgs { get; set; }
    }
}