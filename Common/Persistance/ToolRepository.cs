using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.Persistance
{
    public sealed class ToolRepository
    {
        private static readonly ILogger _logger = Log.ForContext<ToolRepository>();

        public ToolRepository()
        {

        }

        public IEnumerable<ToolStatus> GetAllStatuses()
        {
            var wazuhStatus = GetStatus(ToolName.EndpointDecetionAndResponse);
            var sysmonStatus = GetStatus(ToolName.AdvanceTelemetry);
            var dBytesStatus = GetStatus(ToolName.EndpointDeception);
            var osQueryStatus = GetStatus(ToolName.UserBehaviorAnalytics);
            var avStatus = GetStatus(ToolName.EndpointProtection);
            var lmpStatus = GetStatus(ToolName.LateralMovementProtection);
            return new[] { wazuhStatus, sysmonStatus, dBytesStatus, osQueryStatus, avStatus, lmpStatus };
        }

        public ToolStatus GetStatus(string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{name}", false)) // False is important!
                {
                    var installStatus = (InstallStatus) Enum.Parse(typeof(InstallStatus), (key?.GetValue("INSTALL_STATUS") as string) ?? "NotFound");
                    var runningStatus = (RunningStatus) Enum.Parse(typeof(RunningStatus), (key?.GetValue("RUNNING_STATUS") as string) ?? "NotFound");

                    return new ToolStatus(name, installStatus, runningStatus);
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"Error in reading tool status. {ex}");
            }

            return new ToolStatus(name, InstallStatus.NotFound, RunningStatus.NotFound);
        }

        public static string GetPropertyByName(string path, string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{path}", false)) // False is important!
                {
                    var value = key?.GetValue(name) as string;
                    return value;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void SetPropertyByName(string path, string name, string value)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{path}", true))
                {
                    key?.SetValue(name, value);
                }
            }
            catch
            {
                _logger.Error($"Error in set registry value:{path} {name} {value}");
            }
        }

        public void CaptureEvent(ToolStatus toolStatus)
        {
            _logger.Information($"{toolStatus}");

            var oldStatus = GetStatus(toolStatus.Name);

            if(oldStatus.InstallStatus != toolStatus.InstallStatus)
            {
                SetPropertyByName(toolStatus.Name, "INSTALL_STATUS", toolStatus.InstallStatus.ToString());
            }

            if (oldStatus.RunningStatus != toolStatus.RunningStatus)
            {
                SetPropertyByName(toolStatus.Name, "RUNNING_STATUS", toolStatus.RunningStatus.ToString());
            }

            if (oldStatus.InstallStatus == toolStatus.InstallStatus && oldStatus.RunningStatus == toolStatus.RunningStatus)
            {
                _logger.Information($"{toolStatus} not changed. Skipping...");
                return;
            }

            _logger.Information($"Status changed. Old: {oldStatus}, New: {toolStatus}");

            var log = new EventLog(Constants.LogGroupName) { Source = Constants.IvsAgentName };

            var eventInstance = new EventInstance(toolStatus.GetHashCode(), 0, EventLogEntryType.Information);

            log.WriteEvent(eventInstance, $"{toolStatus.Name} Install: {toolStatus.InstallStatus}, Running: {toolStatus.RunningStatus}");
        }
    }
}
