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
                    Name = ToolName.Wazuuh;
                    break;
                case 2:
                    Name = ToolName.Dbytes;
                    break;
                case 3:
                    Name = ToolName.OsQuery;
                    break;
                case 4:
                    Name = ToolName.Sysmon;
                    break;
                case 5:
                    Name = ToolName.Av;
                    break;
                case 6:
                    Name = ToolName.Lmp;
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
                case ToolName.Wazuuh:
                    code += 100;
                    break;
                case ToolName.Dbytes:
                    code += 200;
                    break;
                case ToolName.OsQuery:
                    code += 300;
                    break;
                case ToolName.Sysmon:
                    code += 400;
                    break;
                case ToolName.Av:
                    code += 500;
                    break;
                case ToolName.Lmp:
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
