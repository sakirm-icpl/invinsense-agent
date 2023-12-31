using Common.Core;

namespace ToolManager.Models
{
    public class InstallType : StringEnumeration
    {
        public static InstallType Installer = new InstallType("INSTALLER", "Installer Based");
        public static InstallType Executable = new InstallType("EXECUTABLE", "Executable based");
        public static InstallType Script = new InstallType("SCRIPT", "Script Based");
        public static InstallType Portable = new InstallType("PORTABLE", "Portable Compressed Folder");

        public InstallType(string id, string name) : base(id, name)
        {
        }
    }
}