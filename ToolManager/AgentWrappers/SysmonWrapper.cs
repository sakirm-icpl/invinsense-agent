using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace ToolManager.AgentWrappers
{
    /// <summary>
    /// https://www.blumira.com/enable-sysmon/
    /// 
    /// </summary>
    public static class SysmonWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(SysmonWrapper));

        public static int Verify(bool isInstall = false)
        {
            try
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

                var exePath = CommonUtils.GetAbsoletePath("..\\artifacts\\Sysmon64.exe");

                var logPath = CommonUtils.DataFolder + "\\sysmonInstall.log";

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

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();


                _logger.Information("SYSMON Installation started...");

                installerProcess.WaitForExit();

                var exitCode = installerProcess.ExitCode;

                if (exitCode == 0)
                {
                    _logger.Information("SYSMON installation completed");
                    return 0;
                }
                else
                {
                    _logger.Information($"SYSMON installation fault: {exitCode}");
                    return exitCode;
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
            _logger.Information($"SYSMON installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"SYSMON installation error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("SYSMON installation process exited.");
        }

        public static int Remove()
        {
            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Sysmon64");

                if (ctl == null)
                {
                    _logger.Information($"SYSMON not found. Skipping");
                    return 0;
                }

                _logger.Information("SYSMON found. Preparing Uninstallation");

                var exePath = CommonUtils.GetAbsoletePath("C:\\Windows\\Sysmon64.exe");

                _logger.Information($"PATH: {exePath}");

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = $"-accepteula -u",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };

                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();


                _logger.Information("SYSMON Uninstallation started...");

                installerProcess.WaitForExit();

                var exitCode = installerProcess.ExitCode;

                if (exitCode == 0)
                {
                    _logger.Information("SYSMON Uninstallation completed");
                }
                else
                {
                    _logger.Information($"SYSMON Uninstallation fault: {exitCode}");
                }

                return exitCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }
        }
    }
}
