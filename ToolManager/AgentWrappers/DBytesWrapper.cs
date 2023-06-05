using Common.Utils;
using ToolManager.MsiWrapper;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Common.Persistance;
using System.IO;

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
                    _logger.Information($"ENDPOINT_DECEPTION found with status: {ctl.Status}");
                    return 0;
                }

                if (ctl == null && !isInstall)
                {
                    _logger.Information("ENDPOINT_DECEPTION not found and set for skip.");
                    return -1;
                }

                var skipInstall = ToolRepository.CanSkipMonitoring(ToolName.EndpointDeception);

                if (skipInstall)
                {
                    _logger.Information("ENDPOINT_DECEPTION not found and set for skip by configuration.");
                    return -1;
                }
                _logger.Information("ENDPOINT_DECEPTION not found. Preparing installation" );

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }
                _logger.Information("ENDPOINT_DECEPTION installation is ready");

                var msiPath = Path.Combine(CommonUtils.ArtifactsFolder, "DeceptiveBytes.EPS.x64.msi");
                var logPath = Path.Combine(CommonUtils.LogsFolder, "dbytesInstall.log");

                _logger.Information($"DBytes msiPath {msiPath}");
                _logger.Information($"DBytes logPath {logPath}");

                var serverIp = ToolRepository.GetPropertyByName(ToolName.EndpointDeception, "SERVER_ADDR");
                var apiKey = ToolRepository.GetPropertyByName(ToolName.EndpointDeception, "APIKEY");

                _logger.Information($"DBytes's ServerIp {serverIp}");
                _logger.Information($"DBytes's ApiKey {apiKey}");

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

                _logger.Information("ENDPOINT_DECEPTION Installation started...");

                installerProcess.WaitForExit();

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("ENDPOINT_DECEPTION installation completed");
                    File.Delete(msiPath);
                }
                else
                {
                    _logger.Information($"ENDPOINT_DECEPTION installation fault: {installerProcess.ExitCode}");
                }

                return installerProcess.ExitCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,new { DbytesError = $"{ex.Message}" });
                return 1;
            }
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"ENDPOINT_DECEPTION installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"ENDPOINT_DECEPTION error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information($"ENDPOINT_DECEPTION process exited.");
        }

        public static int Remove()
        {
            try
            {
                bool status=false;
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DBytesService");

                if (ctl == null)
                {
                    _logger.Information($"ENDPOINT_DECEPTION not found and set for skip.");
                    return -1;
                }
                _logger.Information("ENDPOINT_DECEPTIONfound. Preparing uninstallation");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Information("MSI Installer is not free.");
                    return 1618;
                }
                _logger.Information("ENDPOINT_DECEPTION Uninstallation is ready");
                  
                if(Verify(true)==0)
                {
                    status = MsiPackageWrapper.Uninstall("Deceptive Bytes - Endpoint Deception");
                }  
                    
                return status ? 0 : 1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,new { DbytesError = $"{ex.Message}" });
                return 1;
            }
        }
    }
}
