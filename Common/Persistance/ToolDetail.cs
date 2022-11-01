namespace Common.Persistance
{
    public class ToolDetail
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string AppName { get; set; }

        public bool IsService { get; set; }

        public bool IsMsi { get; set; }

        public string InstallScript { get; set; }

        public string UnInstallScript { get; set; }

        public InstallStatus InstallStatus { get; set; }

        public RunningStatus RunningStatus { get; set; }
    }

    public enum InstallStatus
    {
        NotFound, Outdated, Error, Installed 
    }

    public enum RunningStatus
    {
        NotFound, Stopped, Error, Warning, Running
    }

    public class ToolStatus
    {
        public string Name { get; set; }

        public InstallStatus InstallStatus { get; set; }

        public RunningStatus RunningStatus { get; set; }
    }

    public struct ToolName
    {
        public const string Wazuuh = "WAZUH";
        public const string Dbytes = "DBYTES";
        public const string Sysmon = "SYSMON";
        public const string OsQuery = "OSQUERY";
        public const string Av = "AV";
        public const string Lmp = "LMP";
    }
}
