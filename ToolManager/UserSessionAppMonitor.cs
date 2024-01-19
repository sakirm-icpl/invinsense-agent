using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToolManager
{
    public sealed class UserSessionAppMonitor
    {
        private readonly ILogger _logger = Log.ForContext<UserSessionAppMonitor>();
        private static UserSessionAppMonitor instance = null;
        private static readonly object padlock = new object();
        
        private readonly string appName;
        private readonly string appPath;

        private Process monitoredProcess = null; // Process being monitored

        private UserSessionAppMonitor()
        {
            appName = "IvsTray";
            var destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Infopercept", "IvsTray");
            appPath = Path.Combine(destinationFolder, $"{appName}.exe");
        }

        public static UserSessionAppMonitor Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new UserSessionAppMonitor();
                    }
                    return instance;
                }
            }
        }

        public void StartMonitoring()
        {
            _logger.Information($"Starting monitoring application: {appName}");

            // Find the process and start monitoring
            monitoredProcess = Process.GetProcessesByName(appName).FirstOrDefault();
            if (monitoredProcess != null)
            {
                MonitorProcess();
            }
            else
            {
                StartApplication();
            }
        }

        public void StopMonitoring()
        {
            if (monitoredProcess != null && !monitoredProcess.HasExited)
            {
                monitoredProcess.Kill();
            }
        }

        private void MonitorProcess()
        {
            if (monitoredProcess == null) return;

            _logger.Information($"Monitoring application: {monitoredProcess.ProcessName}");

            monitoredProcess.EnableRaisingEvents = true;
            monitoredProcess.Exited += (sender, args) =>
            {
                _logger.Warning($"Application {appName} exited. Restarting...");
                StartApplication();
            };
        }

        private void StartApplication()
        {
            _logger.Information($"Starting application: {appPath}");

            if(ProcessExtensions.RunInActiveUserSession(null, appPath))
            {
                MonitorProcess();
            }
            else
            {
                _logger.Error($"Failed to start application: {appPath}");
            }
        }
    }
}
