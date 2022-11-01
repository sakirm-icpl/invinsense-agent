﻿namespace Common.Persistance
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
}
