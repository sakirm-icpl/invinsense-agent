﻿using Common.Utils;
using IvsAgent.Monitor;
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
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");
            
            if(ctl != null)
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

            try
            {
                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;

                installerProcess.Start();


                _logger.Information("OSQUERY Installation started...");

                installerProcess.WaitForExit();

                _logger.Information($"Process Exit Code: {installerProcess.ExitCode}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 1;
            }

            _logger.Information("OSQUERY installation completed");

            return 0;
        }

        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"OSQUERY installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Error($"OSQUERY installation error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("OSQUERY installation completed");
        }
    }
}
