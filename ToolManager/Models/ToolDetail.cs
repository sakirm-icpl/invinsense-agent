using ToolManager.Models;

namespace Common.Persistence
{
    public class ToolDetail
    {
        public string Name { get; set; }

        public ToolGroup ToolGroup { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}