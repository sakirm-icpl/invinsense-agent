using Newtonsoft.Json;

namespace Common.Models
{
    public class UpgradeInstruction
    {
        [JsonProperty("minVersion")]
        public string MinVersion { get; set; }

        [JsonProperty("unInstallBeforeUpgrade")]
        public bool UnInstallBeforeUpgrade { get; set; }

        public override string ToString()
        {
            return $"MinVersion: {MinVersion}, UnInstallBeforeUpgrade: {UnInstallBeforeUpgrade}";
        }
    }
}