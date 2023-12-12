using System.Collections.Generic;

namespace Common.Persistence
{
    public class ToolDescriptor
    {
        public string Name { get; set; }

        public ToolGroup ToolGroup { get; set; }

        public string InstallationPath { get; set; }

        public string[] ExecutableFiles { get; set; }

        public string[] ServiceNames { get; set; }

        public ProcessType ProcessType { get; set; }

        public InstallType InstallType { get; set; }

        public InstallStatus InstallStatus { get; set; }

        public List<string> InstallParameters { get; set; }

        public List<string> UninstallParameters { get; set; }

        public RunningStatus RunningStatus { get; set; }

        public bool IsActive { get; set; }

        public void AddInstallParameter(string parameter)
        {
            InstallParameters.Add(parameter);
        }

        public void AddUninstallParameter(string parameter)
        {
            UninstallParameters.Add(parameter);
        }
    }
}