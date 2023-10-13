namespace Common.Persistence
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

        public bool IsInstalled { get; set; }

        public bool IsActive { get; set; }
    }
}
