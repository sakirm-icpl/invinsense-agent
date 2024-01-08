using Common.Models;
using System.ServiceProcess;

namespace Common.RegistryHelpers
{
    public static class ServiceStatusMapper
    {
        public static ToolStatus Map(ToolGroup toolGroup, ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.StopPending:
                case ServiceControllerStatus.PausePending:
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.Running:
                    return new ToolStatus(toolGroup.Id, InstallStatus.Installed, RunningStatus.Running);
                case ServiceControllerStatus.ContinuePending:
                case ServiceControllerStatus.Stopped:
                case ServiceControllerStatus.StartPending:
                    return new ToolStatus(toolGroup.Id, InstallStatus.Installed, RunningStatus.Stopped);
                default:
                    return new ToolStatus(toolGroup.Id, InstallStatus.Unknown, RunningStatus.Unknown);
            }
        }

        public static RunningStatus Map(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.StopPending:
                case ServiceControllerStatus.PausePending:
                case ServiceControllerStatus.Running:
                    return RunningStatus.Running;
                case ServiceControllerStatus.Stopped:
                case ServiceControllerStatus.StartPending:
                    return RunningStatus.Stopped;
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.ContinuePending:
                    return RunningStatus.Warning;
                default:
                    return RunningStatus.Unknown;
            }
        }
    }
}
