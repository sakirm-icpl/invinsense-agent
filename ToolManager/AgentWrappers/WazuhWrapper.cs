using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Xml;
using Common.Persistance;
using ToolManager.MsiWrapper;
using System.IO;
using System.Text;

namespace ToolManager.AgentWrappers
{
    public static class WazuhWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(WazuhWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
            {
                _logger.Information("Verifying END_POINT_DETECTION_AND_RESPONSE");

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                if (ctl != null)
                {
                    _logger.Information($"END_POINT_DETECTION_AND_RESPONSE found with status: {ctl.Status}");

                    CopyActiveResponses();

                    return 0;
                }

                _logger.Information("END_POINT_DETECTION_AND_RESPONSE not found.");

                if (ctl == null && !isInstall)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE set for skip.");
                    return -1;
                }

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("END_POINT_DETECTION_AND_RESPONSE installation is ready");

                var msiPath = Path.Combine(CommonUtils.ArtifactsFolder, "wazuh-agent-4.4.2-1.msi");

                var logPath = Path.Combine(CommonUtils.LogsFolder, "wazuhInstall.log");

                _logger.Information($"Wazuh's msiPath {msiPath}");
                _logger.Information($"Wazuh's logPath {logPath}");

                var inputParameterBuilder = new StringBuilder();

                var managerIp = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "MANAGER_ADDR");
                _logger.Information($"Wazuh's ManagerIp {managerIp}");
                inputParameterBuilder.Append($"WAZUH_MANAGER=\"{managerIp}\"");
                inputParameterBuilder.Append(" ");

                var registrationIp = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_SERVER_ADDR");
                _logger.Information($"Wazuh's RegistrationIP {registrationIp}");
                inputParameterBuilder.Append($"WAZUH_REGISTRATION_SERVER=\"{registrationIp}\"");
                inputParameterBuilder.Append(" ");

                var agentGroup = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "AGENT_GROUP");
                _logger.Information($"Wazuh's AgentGroup {agentGroup}");
                inputParameterBuilder.Append($"WAZUH_AGENT_GROUP=\"{agentGroup}\"");
                inputParameterBuilder.Append(" ");

                var authType = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_TYPE");

                if (authType == "PASSWORD")
                {
                    var registrationPassword = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_PASSWORD");
                    _logger.Information($"Wazuh's RegistrationPassword {registrationPassword}");
                    inputParameterBuilder.Append($"WAZUH_REGISTRATION_PASSWORD=\"{registrationPassword}\"");
                    inputParameterBuilder.Append(" ");
                }

                if(authType == "CERTIFICATE")
                {
                    var certificatePath = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_CERTIFICATE");
                    _logger.Information($"Wazuh's Certificate File Path {certificatePath}");
                    inputParameterBuilder.Append($"WAZUH_REGISTRATION_CERTIFICATE=\"{certificatePath}\"");
                    inputParameterBuilder.Append(" ");

                    var keyPath = ToolRepository.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_KEY");
                    _logger.Information($"Wazuh's Certificate Key File Path {keyPath}");
                    inputParameterBuilder.Append($"WAZUH_REGISTRATION_KEY=\"{keyPath}\"");
                    inputParameterBuilder.Append(" ");
                }

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1 {inputParameterBuilder}",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();

                _logger.Information("END_POINT_DETECTION_AND_RESPONSE Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE installation completed");

                    _logger.Information("Copying local_internal_options.conf file to wazuh installed directory");

                    File.Copy(Path.Combine(CommonUtils.ArtifactsFolder, "local_internal_options.conf"), "C:\\Program Files (x86)\\ossec-agent\\local_internal_options.conf", true);

                    _logger.Information("Enable osquery for END_POINT_DETECTION_AND_RESPONSE");

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

                    CopyActiveResponses();

                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE is ready to start...");

                    ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                    if (ctl == null)
                    {
                        _logger.Information("END_POINT_DETECTION_AND_RESPONSE installed but service not registered. Please check installation logs.");
                    }
                    else
                    {
                        ctl.Start();
                    }

                    File.Delete(Path.Combine(CommonUtils.ArtifactsFolder, "local_internal_options.conf"));
                    File.Delete(Path.Combine(CommonUtils.ArtifactsFolder, "full-scan.exe"));
                    File.Delete(Path.Combine(CommonUtils.ArtifactsFolder, "quick-scan.exe"));
                    File.Delete(msiPath);

                    return 0;
                }
                else
                {
                    _logger.Information($"END_POINT_DETECTION_AND_RESPONSE installation fault: {installerProcess.ExitCode}");
                    return installerProcess.ExitCode;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }

        private static void CopyActiveResponses()
        {
            _logger.Information("Copying active response scripts to wazuh installed directory");

            if(File.Exists(Path.Combine(CommonUtils.ArtifactsFolder, "full-scan.exe")))
            {
                File.Copy(Path.Combine(CommonUtils.ArtifactsFolder, "full-scan.exe"), "C:\\Program Files (x86)\\ossec-agent\\active-response\\bin\\full-scan.exe", true);
            }

            if (File.Exists(Path.Combine(CommonUtils.ArtifactsFolder, "quick-scan.exe")))
            {
                File.Copy(Path.Combine(CommonUtils.ArtifactsFolder, "quick-scan.exe"), "C:\\Program Files (x86)\\ossec-agent\\active-response\\bin\\quick-scan.exe", true);
            }
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"END_POINT_DETECTION_AND_RESPONSE installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"END_POINT_DETECTION_AND_RESPONSE error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("END_POINT_DETECTION process exited.");
        }

        public static int Remove()
        {
            try
            {
                bool status = false;
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                if (ctl == null)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE not found. Skipping...");
                    return -1;
                }
                _logger.Information("END_POINT_DETECTION_AND_RESPONSE found. Preparing uninstallation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }
                _logger.Information("END_POINT_DETECTION_AND_RESPONSE Uninstallation is ready");

                //Checking if file is exists or not
                if (Verify(true) == 0)
                {
                    status = MsiPackageWrapper.Uninstall("Wazuh Agent");
                }

                return status ? 0 : 1;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { WazuhError = $"{ex.Message}" });
                return 1;
            }
        }
    }
}
