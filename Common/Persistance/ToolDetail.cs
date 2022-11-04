namespace Common.Persistance
{
    public class ToolDetail
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AppName { get; set; }

        public string SetupFileName { get; set; }

        public bool IsService { get; set; }

        public bool IsMsi { get; set; }

        public bool IsWin64 { get; set; }

        public string InstallArgs { get; set; }

        public string UpdateArgs { get; set; }

        public string UninstallArgs { get; set; }

        public InstallStatus InstallStatus { get; set; }

        public RunningStatus RunningStatus { get; set; }
    }
}
