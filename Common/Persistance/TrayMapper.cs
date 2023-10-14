namespace Common.Persistence
{
    public static class TrayMapper
    {
        public static ToolStatus MapFromEventId(long eventId)
        {
            var toolName = "";
            var installStatus = InstallStatus.NotFound;
            var runningStatus = RunningStatus.NotFound;

            int toolId = (int)(eventId / 100);

            if (toolId == 0 || toolId > 6)
            {
                return new ToolStatus(toolName, installStatus, runningStatus);
            }

            switch (toolId)
            {
                case 1:
                    toolName = ToolName.EndpointDetectionAndResponse;
                    break;
                case 2:
                    toolName = ToolName.EndpointDeception;
                    break;
                case 3:
                    toolName = ToolName.UserBehaviorAnalytics;
                    break;
                case 4:
                    toolName = ToolName.AdvanceTelemetry;
                    break;
                case 5:
                    toolName = ToolName.EndpointProtection;
                    break;
                case 6:
                    toolName = ToolName.LateralMovementProtection;
                    break;
                default:
                    break;
            }

            installStatus = (InstallStatus)((eventId - toolId * 100) / 10);
            runningStatus = (RunningStatus)(eventId - toolId * 100 - (int)installStatus * 10);

            return new ToolStatus(toolName, installStatus, runningStatus);
        }
    }
}
