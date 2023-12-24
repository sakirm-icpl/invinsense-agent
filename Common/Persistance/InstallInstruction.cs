using System.Collections.Generic;

namespace Common.Persistence
{
    public class InstallInstruction
    {
        public string Name { get; set; }

        public string DownloadUrl { get; set; }

        public string FileHash { get; set; }

        public string InstallationPath { get; set; }

        public InstallType InstallType { get; set; }

        public List<string> InstallParameters { get; set; }

        public List<string> UninstallParameters { get; set; }
    }
}