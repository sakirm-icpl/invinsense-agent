using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace IvsAgent.AgentWrappers
{
    internal static class SysmonWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(SysmonWrapper));

        public static int Verify(bool isInstall = false)
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Sysmon64");

            if (ctl != null)
            {
                _logger.Information($"SYSMON found with status: {ctl.Status}");
                return 0;
            }

            if (ctl == null && !isInstall)
            {
                _logger.Information("SYSMON not found and set for skip.");
                return -1;
            }

            _logger.Information("SYSMON not found. Preparing installation");

            var exePath = CommonUtils.GetAbsoletePath("artifacts\\Sysmon64.exe");

            var logPath = CommonUtils.GetAbsoletePath("sysmonInstall.log");

            _logger.Information($"PATH: {exePath}, Log: {logPath}");

            Process installerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"-accepteula -i",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = CommonUtils.RootFolder
                }
            };

            try
            {
                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();


                _logger.Information("SYSMON Installation started...");

                installerProcess.WaitForExit();

                _logger.Information($"Process Exit Code: {installerProcess.ExitCode}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }

            _logger.Information("SYSMON installation completed");
            return 0;
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"SYSMON installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"SYSMON installation error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("SYSMON installation completed");
        }
    }
}
