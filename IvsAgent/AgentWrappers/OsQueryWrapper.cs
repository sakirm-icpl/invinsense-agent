using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace IvsAgent.AgentWrappers
{
    internal static class OsQueryWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(OsQueryWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");

                if (ctl != null)
                {
                    _logger.Information($"OSQUERY found with status: {ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("OSQUERY not found and set for skip.");
                    return -1;
                }

                _logger.Information("OSQUERY not found. Preparing installation");

                if(!MsiWrapper.MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("OSQUERY installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("artifacts\\osquery-5.5.1.msi");

                var logPath = CommonUtils.GetAbsoletePath("osqueryInstall.log");

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();


                _logger.Information("OSQUERY Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("OSQUERY installation completed");
                    return 0;
                }
                else
                {
                    _logger.Information($"OSQUERY installation fault: {installerProcess.ExitCode}");
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
            _logger.Information($"OSQUERY installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"OSQUERY error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("OSQUERY process exited.");
        }

        public static int Remove()
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");

                if (ctl == null)
                {
                    _logger.Information("OSQUERY not found and set for skip.");
                    return -1;
                }

                _logger.Information("OSQUERY not found. Preparing uninstallation");

                if (!MsiWrapper.MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("OSQUERY installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("artifacts\\osquery-5.5.1.msi");

                var logPath = CommonUtils.GetAbsoletePath("osqueryInstall.log");

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


                _logger.Information("OSQUERY uninstall started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("OSQUERY uninstall completed");
                    return 0;
                }
                else
                {
                    _logger.Information($"OSQUERY uninstall fault: {installerProcess.ExitCode}");
                    return installerProcess.ExitCode;

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }
    }
}
