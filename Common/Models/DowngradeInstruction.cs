using Newtonsoft.Json;

namespace Common.Models
{
    public class DowngradeInstruction
    {
        [JsonProperty("maxVersion")] 
        public string MaxVersion { get; set; }

        [JsonProperty("unInstallBeforeDowngrade")]
        public bool UnInstallBeforeDowngrade { get; set; }

        public override string ToString()
        {
            return $"MaxVersion: {MaxVersion}, UninstallBeforeDowngrade: {UnInstallBeforeDowngrade}";
        }
    }
}