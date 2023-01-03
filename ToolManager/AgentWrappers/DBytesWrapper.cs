using Common.Utils;
using ToolManager.MsiWrapper;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Common.Persistance;

namespace ToolManager.AgentWrappers
{
    public static class DBytesWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(DBytesWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DBytesService");

                if (ctl != null)
                {
                    _logger.Information($"END_POINT_DECEPTION found with status: {ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("END_POINT_DECEPTION not found and set for skip.");
                    return -1;
                }

                _logger.Information("END_POINT_DECEPTION not found. Preparing installation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("END_POINT_DECEPTION installation is ready");

                var msiPath = CommonUtils.GetAbsoletePath("..\\artifacts\\DeceptiveBytes.EPS.x64.msi");

                var logPath = CommonUtils.DataFolder + "\\dbytesInstall.log";

                _logger.Information($"PATH: {msiPath}, Log: {logPath}");

                var serverIp = ToolRepository.GetPropertyByName(ToolName.Dbytes, "SERVER_ADDR");
                var apiKey = ToolRepository.GetPropertyByName(ToolName.Dbytes, "APIKEY");

                _logger.Information($"ServerIP: {serverIp}, ApiKey: {apiKey}");

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

                _logger.Information("END_POINT_DECEPTION Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("END_POINT_DECEPTION installation completed");
                }
                else
                {
                    _logger.Information($"END_POINT_DECEPTION installation fault: {installerProcess.ExitCode}");
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
            _logger.Information($"END_POINT_DECEPTION installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"END_POINT_DECEPTION error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("END_POINT_DECEPTION process exited.");
        }

        public static int Remove()
        {
            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DBytesService");

                if (ctl == null)
                {
                    _logger.Information("END_POINT_DETECTION not found and set for skip.");
                    return -1;
                }

                _logger.Information("END_POINT_DETECTION found. Preparing uninstallation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("END_POINT_DETECTION Uninstallation is ready");

                var logPath = CommonUtils.DataFolder + "\\dytesInstall.log";

                var status = MsiPackageWrapper.Uninstall("Deceptive Bytes - Active Endpoint Deception", logPath);

                
                return status ? 0 : 1;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }
    }
}
