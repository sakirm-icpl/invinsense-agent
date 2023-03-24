using Serilog;
using System.Diagnostics;
using System.ServiceProcess;
using Common.Utils;
using System.Xml;

namespace Wazuh
{
    public static class Wazuh
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(Wazuh));
        public static void Main(string[] args)
        {
            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WazuhSvc");
                if (ctl != null)
                {
                    _logger.Information($"END_POINT_DETECTION_AND_RESPONSE found with status: {ctl.Status}");

                }
                if (ctl == null)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE not found and set for skip.");

                }
                _logger.Information("END_POINT_DETECTION_AND_RESPONSE not found and set for skip.");

                _logger.Information("END_POINT_DETECTION_AND_RESPONSE installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("D:\\invinsense-agent\\artifacts\\wazuh\\wazuh-agent-4.3.10-1.msi");

                var logPath = CommonUtils.DataFolder + "\\wazuhInstall.log";

                _logger.Information($"Wazuh's msiPath {msiPath}");
                _logger.Information($"Wazuh's logPath {logPath}");

                //var managerIp = ToolRepository.GetPropertyByName(ToolName.EndpointDecetionAndResponse, "MANAGER_ADDR");
                //var registrationIp = ToolRepository.GetPropertyByName(ToolName.EndpointDecetionAndResponse, "REGISTRATION_SERVER_ADDR");
                //var agentGroup = ToolRepository.GetPropertyByName(ToolName.EndpointDecetionAndResponse, "AGENT_GROUP");

                // _logger.Information($"Wazuh's ManagerIp {managerIp}");
                //_logger.Information($"Wazuh's RegistrationIP {registrationIp}");
                //_logger.Information($"Wazuh's AgentGroup {agentGroup}");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1 WAZUH_MANAGER=\"172.17.14.101\" WAZUH_REGISTRATION_SERVER=\"172.17.14.101\" WAZUH_AGENT_GROUP=\"default\"",
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

                installerProcess.WaitForExit(200000);

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("END_POINT_DETECTION_AND_RESPONSE installation completed");

                    _logger.Information("Copying local_internal_options.conf file to wazuh installed directory");

                    System.IO.File.Copy(CommonUtils.GetAbsoletePath("D:\\invinsense-agent\\artifacts\\wazuh\\local_internal_options.conf"), "C:\\Program Files (x86)\\ossec-agent\\local_internal_options.conf", true);

                    Thread.Sleep(1000);

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
                }
                else
                {
                    _logger.Information($"END_POINT_DETECTION_AND_RESPONSE installation fault: {installerProcess.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
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
    }
}