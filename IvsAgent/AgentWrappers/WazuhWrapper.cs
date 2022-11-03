using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace IvsAgent.AgentWrappers
{
    internal static class WazuhWrapper
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

                if(!MsiWrapper.MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("WAZUH installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\wazuh-agent-4.3.9-1.msi");

                var logPath = CommonUtils.GetAbsoletePath("..\\artifacts\\wazuhInstall.log");

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                var managerIp = ToolProperties.GetPropertyByName("WAZUH_MANAGER");
                var registrationIp = ToolProperties.GetPropertyByName("WAZUH_REGISTRATION_SERVER");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1 WAZUH_MANAGER={managerIp} WAZUH_REGISTRATION_SERVER={registrationIp}",
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
    }
}
