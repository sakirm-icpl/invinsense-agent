using System.Collections.Generic;

namespace Common.Persistence
{
    public class ToolDescriptor
    {
        public string Name { get; set; }

        public ToolGroup ToolGroup { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string InstallationPath { get; set; }

        public List<ExecutableFile> ExecutableFiles { get; set; }

        public List<WinService> Services { get; set; }

        public bool IsActive { get; set; }
    }
}