namespace Common.Persistance
{
    public class ToolStatus
    {
        public ToolStatus(string name, InstallStatus installStatus, RunningStatus runningStatus)
        {
            Name = name;
            InstallStatus = installStatus;
            RunningStatus = runningStatus;
        }

        public ToolStatus(long eventId)
        {
            int toolId = (int)(eventId / 100);

            if (toolId == 0 || toolId > 6)
            {
                Name = "";
                InstallStatus = InstallStatus.NotFound;
                RunningStatus = RunningStatus.NotFound;
                return;
            }

            switch (toolId)
            {
                case 1:
                    Name = ToolName.EndpointDecetionAndResponse;
                    break;
                case 2:
                    Name = ToolName.EndpointDeception;
                    break;
                case 3:
                    Name = ToolName.UserBehaviorAnalytics;
                    break;
                case 4:
                    Name = ToolName.AdvanceTelemetry;
                    break;
                case 5:
                    Name = ToolName.EndpointProtection;
                    break;
                case 6:
                    Name = ToolName.LateralMovementProtection;
                    break;
                default:
                    break;
            }

            InstallStatus = (InstallStatus) ((eventId - toolId * 100) / 10);
            RunningStatus = (RunningStatus) (eventId - toolId * 100 - (int) InstallStatus * 10);
        }

        public string Name { get; }

        public InstallStatus InstallStatus { get; }

        public RunningStatus RunningStatus { get; }

        public override int GetHashCode()
        {
            var code = 0;
            switch (Name)
            {
                case ToolName.EndpointDecetionAndResponse:
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

        /*public override string ToString()
        {
            return $"{Name} Install: {InstallStatus} Running: {RunningStatus}";
        }*/
    }
}
