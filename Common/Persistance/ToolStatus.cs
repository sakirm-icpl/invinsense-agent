using Newtonsoft.Json;

namespace Common.Persistence
{
    public class ToolStatus
    {
        [JsonConstructor]
        public ToolStatus(string name, InstallStatus installStatus, RunningStatus runningStatus)
        {
            Name = name;
            InstallStatus = installStatus;
            RunningStatus = runningStatus;
        }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("installStatus")]
        public InstallStatus InstallStatus { get; }

        [JsonProperty("runningStatus")]
        public RunningStatus RunningStatus { get; }

        public override int GetHashCode()
        {
            var code = 0;
            switch (Name)
            {
                case ToolName.EndpointDetectionAndResponse:
                    code += 100;
                    break;
                case ToolName.EndpointDeception:
                    code += 200;
                    break;
                case ToolName.UserBehaviorAnalytics:
                    code += 300;
                    break;
                case ToolName.AdvanceTelemetry:
                    code += 400;
                    break;
                case ToolName.EndpointProtection:
                    code += 500;
                    break;
                case ToolName.LateralMovementProtection:
                    code += 600;
                    break;
                default:
                    break;
            }

            code += (int)InstallStatus * 10;
            code += (int)RunningStatus;

            return code;
        }

        public override string ToString()
        {
            return $"{Name} Install: {InstallStatus} Running: {RunningStatus}";
        }
    }
}
