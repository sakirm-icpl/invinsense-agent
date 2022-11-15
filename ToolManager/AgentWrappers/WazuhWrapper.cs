using Common.Utils;
using ToolManager.MsiWrapper;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Xml;
using Common.Persistance;

namespace ToolManager.AgentWrappers
{
    public static class WazuhWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(WazuhWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                if (ctl != null)
                {
                    _logger.Information($"WAZUH found with status: {ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("WAZUH not found and set for skip.");
                    return -1;
                }

                _logger.Information("WAZUH not found. Preparing installation");

                if (!MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("WAZUH installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\wazuh-agent-4.3.9-1.msi");

                var logPath = CommonUtils.DataFolder + "\\wazuhInstall.log";

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                var managerIp = ToolRepository.GetPropertyByName(ToolName.Wazuuh, "MANAGER_ADDR");
                var registrationIp = ToolRepository.GetPropertyByName(ToolName.Wazuuh, "REGISTRATION_SERVER_ADDR");
                var agentGroup = ToolRepository.GetPropertyByName(ToolName.Wazuuh, "AGENT_GROUP");
                
                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1 WAZUH_MANAGER=\"{managerIp}\" WAZUH_REGISTRATION_SERVER=\"{registrationIp}\" WAZUH_AGENT_GROUP=\"{agentGroup}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();

                _logger.Information("WAZUH Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("WAZUH installation completed");

                    _logger.Information("Copying local_internal_options.conf file to wazuh installed directory");
                    System.IO.File.Copy(CommonUtils.GetAbsoletePath("..\\artifacts\\local_internal_options.conf"), "C:\\Program Files (x86)\\ossec-agent\\local_internal_options.conf", true);

                    _logger.Information("enable osquery for wazuh");
                    var confFile = "C:\\Program Files (x86)\\ossec-agent\\ossec.conf";
                    XmlDocument document = new XmlDocument();
                    document.Load(confFile);
                    XmlNodeList osQueryDisableNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/disabled");
                    if (osQueryDisableNodeItems.Count > 0)
                    {
                        osQueryDisableNodeItems[0].InnerText = "no";
                    }

                    XmlNodeList osQueryRunDaemonNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/run_daemon");
                    if (osQueryRunDaemonNodeItems.Count > 0)
                    {
                        osQueryRunDaemonNodeItems[0].InnerText = "no";
                    }
                    document.Save(confFile);

                    _logger.Information("wazuh is ready to start...");

                    ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                    if (ctl == null)
                    {
                        _logger.Information($"WAZUH installed but service not registered. Please check installation logs.");
                    }
                    else
                    {
                        ctl.Start();
                    }

                    return 0;
                }
                else
                {
                    _logger.Information($"WAZUH installation fault: {installerProcess.ExitCode}");
                    return installerProcess.ExitCode;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"WAZUH installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"WAZUH error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("WAZUH process exited.");
        }

        public static int Remove()
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                if (ctl == null)
                {
                    _logger.Information($"WAZUH not found. Skipping...");
                    return 0;
                }

                _logger.Information("WAZUH found. Preparing uninstallation");

                if (!MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("WAZUH uninstallation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\wazuh-agent-4.3.9-1.msi");

                var logPath = CommonUtils.DataFolder + "\\wazuhInstall.log";

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/X \"{msiPath}\" /QN /l*vx \"{logPath}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();

                _logger.Information("WAZUH Uninstallation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("WAZUH Uninstallation completed");
                }
                else
                {
                    _logger.Information($"WAZUH Uninstallation fault: {installerProcess.ExitCode}");
                }

                return installerProcess.ExitCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }
    }
}
