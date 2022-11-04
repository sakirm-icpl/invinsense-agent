using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace IvsAgent.AgentWrappers
{
    internal static class DBytesWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(DBytesWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DBytesService");

                if (ctl != null)
                {
                    _logger.Information($"DBYTES found with status: {ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("DBYTES not found and set for skip.");
                    return -1;
                }

                _logger.Information("DBYTES not found. Preparing installation");

                if(!MsiWrapper.MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("DBYTES installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\DeceptiveBytes.EPS.x64.msi");

                var logPath = CommonUtils.GetAbsoletePath("..\\artifacts\\dbytesInstall.log");

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                var serverIp = ToolProperties.GetPropertyByName("DBYTES_SERVER");
                var apiKey = ToolProperties.GetPropertyByName("DBYTES_APIKEY");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ALLUSERS=1 /norestart ServerAddress=\"{serverIp}\" ApiKey=\"{apiKey}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();

                _logger.Information("DBYTES Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("DBYTES installation completed");
                }
                else
                {
                    _logger.Information($"DBYTES installation fault: {installerProcess.ExitCode}");
                }

                return installerProcess.ExitCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"DBYTES installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"DBYTES error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("DBYTES process exited.");
        }

        public static int Remove()
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DBytesService");

                if (ctl == null)
                {
                    _logger.Information("DBYTES not found and set for skip.");
                    return -1;
                }

                _logger.Information("DBYTES not found. Preparing uninstallation");

                if (!MsiWrapper.MsiPackage.IsMsiExecFree(TimeSpan.FromSeconds(2)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("DBYTES installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\DeceptiveBytes.EPS.x64.msi");

                var logPath = CommonUtils.GetAbsoletePath("..\\artifacts\\DBYTESInstall.log");

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


                _logger.Information("DBYTES uninstall started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("DBYTES uninstall completed");
                    return 0;
                }
                else
                {
                    _logger.Information($"DBYTES uninstall fault: {installerProcess.ExitCode}");
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
