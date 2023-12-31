using System.Collections.Generic;
using ToolManager.Models;

namespace Common.Persistence
{
    public class InstallInstruction
    {
        public string Name { get; set; }

        public string DownloadUrl { get; set; }

        public string FileHash { get; set; }

        public string RequiredVersion { get; set; }

        public string MinimumVersion { get; set; }

        public string MaximumVersion { get; set; }

        public string InstallationPath { get; set; }

        public InstallType InstallType { get; set; }

        public string WorkingDirectory { get; set; }

        public List<string> InstallArgs { get; set; }

        public List<string> UninstallArgs { get; set; }
    }
}