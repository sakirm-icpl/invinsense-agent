using System.Collections.Generic;
using Common.Persistence;

namespace ToolManager
{
    public static class ToolDescriptors
    {
        public static ToolDescriptor OsQuery = new ToolDescriptor
        {
            Name = "osquery",
            InstallationPath = @"C:\Program Files\osquery",
            ExecutableFiles = new[] { "osqueryi.exe", "osqueryd\\osqueryd.exe" },

            InstallType = InstallType.Installer,
            InstallStatus = InstallStatus.Unknown,
            RunningStatus = RunningStatus.Unknown,

            InstallParameters = new List<string> { "ALLUSERS=1", "ACCEPTEULA=1" },
            IsActive = true,

            ServiceNames = new[] { "osqueryd" },
            ProcessType = ProcessType.Service,
            ToolGroup = ToolGroup.UserBehaviorAnalytics,
            UninstallParameters = new List<string>()
        };

        public static ToolDescriptor Sysmon = new ToolDescriptor
        {
            Name = "sysmon",
            InstallationPath = @"C:\Program Files\Sysmon",
            ExecutableFiles = new[] { "sysmon.exe" },

            InstallType = InstallType.Installer,
            InstallStatus = InstallStatus.Unknown,
            RunningStatus = RunningStatus.Unknown
        };

        public static ToolDescriptor SysmonView = new ToolDescriptor
        {
            Name = "sysmonview",
            InstallationPath = @"C:\Program Files\Sysmon",
            ExecutableFiles = new[] { "SysmonView.exe" },

            InstallType = InstallType.Installer,
            InstallStatus = InstallStatus.Unknown,
            RunningStatus = RunningStatus.Unknown
        };

        public static ToolDescriptor SysmonTools = new ToolDescriptor
        {
            Name = "sysmon-tools",
            InstallationPath = @"C:\Program Files\Sysmon",
            ExecutableFiles = new[] { "SysmonTools.exe" },

            InstallType = InstallType.Installer,
            InstallStatus = InstallStatus.Unknown,
            RunningStatus = RunningStatus.Unknown
        };

        public static ToolDescriptor SysmonConfig = new ToolDescriptor
        {
            Name = "sysmon-config",
            InstallationPath = @"C:\Program Files\Sysmon",
            ExecutableFiles = new[] { "SysmonConfig.exe" },

            InstallType = InstallType.Installer,
            InstallStatus = InstallStatus.Unknown,
            RunningStatus = RunningStatus.Unknown
        };
    }
}