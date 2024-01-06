using Serilog;
using System.Diagnostics;
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

        private UserSessionAppMonitor(string applicationName, string applicationPath)
        {
            appName = applicationName; // Set the application name
            appPath = applicationPath;
            StartMonitoring();
        }

        public static UserSessionAppMonitor Instance(string applicationName, string applicationPath)
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new UserSessionAppMonitor(applicationName, applicationPath);
                }
                return instance;
            }
        }

        private void StartMonitoring()
        {
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
            // Logic to start the application
            if(ProcessExtensions.RunInActiveUserSession(null, appPath))
            {
                MonitorProcess();
            }
        }

        public void StopMonitoring()
        {
            if (monitoredProcess != null && !monitoredProcess.HasExited)
            {
                monitoredProcess.Kill();
            }
        }
    }
}
