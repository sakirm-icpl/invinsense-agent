using Common.Models.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{InstallType} - {InstallerFile}");
            sb.AppendLine("Install Args:");
            foreach (var arg in InstallArgs)
            {
                sb.AppendLine(arg);
            }
            sb.AppendLine("Uninstall Args:");
            foreach (var arg in UninstallArgs)
            {
                sb.AppendLine(arg);
            }
            return sb.ToString();            
        }
    }
}