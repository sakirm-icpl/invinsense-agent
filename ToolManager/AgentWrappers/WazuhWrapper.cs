using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Xml;
using Common.Persistence;
using ToolManager.MsiWrapper;
using System.IO;
using System.Text;
using Common.Mappers;

namespace ToolManager.AgentWrappers
{
    /// <summary>
    /// TODO: Need to create new logic that checks agent install status with currupted files.
    /// </summary>
    public static class WazuhWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(WazuhWrapper));

        public static bool GetInstalledVersion(out Version productVersion)
        {
            productVersion = new Version();

            try
            {
                _logger.Information("Verifying END_POINT_DETECTION_AND_RESPONSE");

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

                if (ctl == null)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE is not present.");
                    return false;
                }

                var filePath = "C:\\Program Files (x86)\\ossec-agent\\VERSION";

                if (!File.Exists(filePath))
                {
                    _logger.Error("END_POINT_DETECTION_AND_RESPONSE VERSION file does not exist.");
                    return false;
                }

                var versionContent = File.ReadLines(filePath).First().Substring(1);

                _logger.Information($"END_POINT_DETECTION_AND_RESPONSE version: {versionContent}");

                productVersion = new Version(versionContent);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return false;
            }
        }

        public static int Install()
        {
            try
            {
                _logger.Information("Checking MSI status for executing installer...");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("END_POINT_DETECTION_AND_RESPONSE installation is ready");

                var msiPath = Path.Combine(CommonUtils.ArtifactsFolder, "wazuh-agent-4.4.1-1.msi");
                var logPath = Path.Combine(CommonUtils.LogsFolder, "wazuhInstall.log");

                _logger.Information($"Wazuh's msiPath {msiPath}");
                _logger.Information($"Wazuh's logPath {logPath}");

                var inputParameterBuilder = new StringBuilder();

                var managerIp = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "MANAGER_ADDR");
                _logger.Information($"Wazuh's ManagerIp {managerIp}");
                inputParameterBuilder.Append($"WAZUH_MANAGER=\"{managerIp}\"");
                inputParameterBuilder.Append(" ");

                var registrationIp = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_SERVER_ADDR");
                _logger.Information($"Wazuh's RegistrationIP {registrationIp}");
                inputParameterBuilder.Append($"WAZUH_REGISTRATION_SERVER=\"{registrationIp}\"");
                inputParameterBuilder.Append(" ");

                //settig wazuh manager and registeration server ip in environment variable
                _logger.Information($"Setting Environment Variables WAZUH_MANAGER=\"{managerIp}\" and REGISTRATION_SERVER=\"{registrationIp}\"");
                Environment.SetEnvironmentVariable("WAZUH_MANAGER", managerIp, EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("WAZUH_REGISTRATION_SERVER", registrationIp, EnvironmentVariableTarget.Machine);

                var agentGroup = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "AGENT_GROUP");
                _logger.Information($"Wazuh's AgentGroup {agentGroup}");
                inputParameterBuilder.Append($"WAZUH_AGENT_GROUP=\"{agentGroup}\"");
                inputParameterBuilder.Append(" ");

                var authType = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_TYPE");

                if (authType == "PASSWORD")
                {
                    var registrationPassword = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_PASSWORD");
                    _logger.Information($"Wazuh's RegistrationPassword {registrationPassword}");
                    inputParameterBuilder.Append($"WAZUH_REGISTRATION_PASSWORD=\"{registrationPassword}\"");
                    inputParameterBuilder.Append(" ");
                }

                if (authType == "CERTIFICATE")
                {
                    var certificatePath = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_CERTIFICATE");
                    _logger.Information($"Wazuh's Certificate File Path {certificatePath}");
                    inputParameterBuilder.Append($"WAZUH_REGISTRATION_CERTIFICATE=\"{certificatePath}\"");
                    inputParameterBuilder.Append(" ");

                    var keyPath = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_KEY");
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
                    //Delete msi file
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

        public static int EdrServiceCheck()
        {
            try
            {
                EnsureCredential();

                EnsureLocalInternalOptions();

                EnsureActiveResponse();

                EnableOsQueryWoodle();

                TryStartService();

                EnsureEnvironmentVariables();

                EnsureIsolationMessage();

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Enable osquery for END_POINT_DETECTION_AND_RESPONSE
        /// </summary>
        private static void EnableOsQueryWoodle()
        {
            _logger.Information("Enable osquery for END_POINT_DETECTION_AND_RESPONSE");
            var confFile = "C:\\Program Files (x86)\\ossec-agent\\ossec.conf";

            if (!File.Exists(confFile))
            {
                _logger.Error("END_POINT_DETECTION_AND_RESPONSE configuration not found.");
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(confFile);
            XmlNodeList osQueryDisableNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/disabled");
            if (osQueryDisableNodeItems.Count > 0 && osQueryDisableNodeItems[0].InnerText != "no")
            {
                osQueryDisableNodeItems[0].InnerText = "no";
            }

            XmlNodeList osQueryRunDaemonNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/run_daemon");
            if (osQueryRunDaemonNodeItems.Count > 0 && osQueryRunDaemonNodeItems[0].InnerText != "no")
            {
                osQueryRunDaemonNodeItems[0].InnerText = "no";
            }

            document.Save(confFile);
        }

        private static void TryStartService()
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");

            if (ctl != null)
            {
                _logger.Information("END_POINT_DETECTION_AND_RESPONSE detected.");

                if (ctl.Status == ServiceControllerStatus.Stopped)
                {
                    _logger.Verbose("END_POINT_DETECTION_AND_RESPONSE Not Running. Trying to start service.");
                    ctl.Start();
                }
            }
        }

        private static void EnsureCredential()
        {
            var authType = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_TYPE");

            if (authType == "PASSWORD")
            {
                var registrationPassword = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_PASSWORD");
                _logger.Information($"Wazuh's RegistrationPassword {registrationPassword}");
                File.WriteAllText("C:\\Program Files (x86)\\ossec-agent\\authd.pass", registrationPassword);
            }
        }

        private static void EnsureLocalInternalOptions()
        {
            _logger.Information("Copying local_internal_options.conf file to wazuh installed directory");

            File.Copy(Path.Combine(CommonUtils.ArtifactsFolder, "local_internal_options.conf"), "C:\\Program Files (x86)\\ossec-agent\\local_internal_options.conf", true);
            File.Delete(Path.Combine(CommonUtils.ArtifactsFolder, "local_internal_options.conf"));
        }

        private static void EnsureActiveResponse()
        {
            _logger.Information("Copying active response scripts to wazuh installed directory");

            var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            var wazuhAgentActiveResponseBinPath = Path.Combine(pf86, "ossec-agent\\active-response\\bin");

            _logger.Information($"Wazuh Agent Active Response Bin Path:{wazuhAgentActiveResponseBinPath}");

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "full-scan.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "full-scan.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "quick-scan.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "quick-scan.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "scheduled-scan.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "scheduled-scan.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "isolation.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "isolation.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "isolation-anydesk.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "isolation-anydesk.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "isolation-rdp.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "isolation-rdp.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "unisolation.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "unisolation.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "unisolation-rdp.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "unisolation-rdp.exe"));

            EnsureFile(Path.Combine(CommonUtils.ArtifactsFolder, "block-domain.exe"), Path.Combine(wazuhAgentActiveResponseBinPath, "block-domain.exe"));
        }

        private static void EnsureFile(string source, string destination)
        {
            if (!File.Exists(source))
            {
                _logger.Error($"File does not exist at source {source}");
                return;
            }

            File.Copy(source, destination, true);
            File.Delete(source);
        }

        private static void EnsureEnvironmentVariables()
        {
            var currentManagerIp = Environment.GetEnvironmentVariable("WAZUH_MANAGER");
            var currentRegistrationIp = Environment.GetEnvironmentVariable("WAZUH_REGISTRATION_SERVER");

            var managerIp = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "MANAGER_ADDR");
            var registrationIp = ToolRegistry.GetPropertyByName(ToolName.EndpointDetectionAndResponse, "REGISTRATION_SERVER_ADDR");
            if (currentRegistrationIp == null && currentRegistrationIp == null)
            {
                Environment.SetEnvironmentVariable("WAZUH_MANAGER", managerIp, EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("WAZUH_REGISTRATION_SERVER", registrationIp, EnvironmentVariableTarget.Machine);
            }
            else
            {
                if (!currentManagerIp.Equals(managerIp) && !currentRegistrationIp.Equals(registrationIp))
                {
                    Environment.SetEnvironmentVariable("WAZUH_MANAGER", managerIp, EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("WAZUH_REGISTRATION_SERVER", registrationIp, EnvironmentVariableTarget.Machine);
                }
            }
        }

        /// <summary>
        /// This is quick implementation to set Isolation message in registry.
        /// </summary>
        private static void EnsureIsolationMessage()
        {
            var (success, error, groups) = ReadGroups();

            if(!success)
            {
                _logger.Error(error);
                return;
            }

            var displayMessage = DisplayMessageMapper.MapNetworkIsolationMessage(groups.Split(','));

            ToolRegistry.SetPropertyByName("I18N", "Groups", groups);
            ToolRegistry.SetPropertyByName("I18N", "IsolationTitle", displayMessage.Title);
            ToolRegistry.SetPropertyByName("I18N", "IsolationMessage", displayMessage.Message);
        }

        private static (bool, string, string) ReadGroups()
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var configPath = Path.Combine(programFiles, "ossec-agent\\ossec.conf");

            if (File.Exists(configPath) == false)
            {
                return (false, "ossec.conf file not found under program files. Please ensure that wazuh agent is installed.", "");
            }

            try
            {
                string xmlContent = File.ReadAllText(configPath);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                XmlNode groupsNode = doc.SelectSingleNode("//groups");
                if (groupsNode != null)
                {
                    string groupsText = groupsNode.InnerText;
                    return (true, "", groupsText);
                }
                else
                {
                    return (false, "The <groups> element was not found.", "");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, "");
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

                status = MsiPackageWrapper.Uninstall("Wazuh Agent");

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
