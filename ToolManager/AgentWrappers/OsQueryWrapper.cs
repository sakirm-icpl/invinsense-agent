using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceProcess;
using ToolManager.MsiWrapper;

namespace ToolManager.AgentWrappers
{
    public static class OsQueryWrapper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(OsQueryWrapper));
        public static int Verify(bool isInstall = false)
        {
            try
            {

                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");

                if (ctl != null)
                {
                    _logger.Information($"OsQueryStatus:{ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("OSQUERY not found and set for skip.");
                    return -1;
                }
                _logger.Information("OSQUERY not found. Preparing installation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("OSQUERY installation is ready");

                var msiPath = Path.Combine(CommonUtils.ArtifactsFolder, "osquery-5.5.1.msi");

                var logPath = Path.Combine(CommonUtils.LogsFolder, "osqueryInstall.log");

                _logger.Information($"OsQuery msiPath {msiPath}");
                _logger.Information($"OsQuery logPath {logPath}");

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

                    _logger.Information("Copying osquery.conf file to osquery installed directory");

                    File.Copy(Path.Combine(CommonUtils.ArtifactsFolder, "osquery.conf"), "C:\\Program Files\\osquery\\osquery.conf", true);

                    _logger.Information("OsQueryStatus", new { OsQueryStatus = "Extract packs to osquery" });

                    ZipFile.ExtractToDirectory(Path.Combine(CommonUtils.ArtifactsFolder, "osquery-packs.zip"), "C:\\Program Files\\osquery\\packs");
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
                _logger.Error($"{ex.Message}");
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
                bool status = false;
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");

                if (ctl == null)
                {
                    _logger.Information("OSQUERY not found and set for skip.");
                    return -1;
                }

                _logger.Information("OSQUERY found. Preparing uninstallation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }

                _logger.Information("OSQUERY Uninstallation is ready");

                //Checking if file is exists or not
                if (Verify(true) == 0)
                {
                    _logger.Information("OSQUERY uninstall started...");
                    status = MsiPackageWrapper.Uninstall("osquery");
                }

                return status ? 0 : 1;

            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }
    }
}
