using Common.ConfigProvider;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using ToolManager;

namespace ToolChecker.Monitor
{
    /// <summary>
    /// TODO: Need to handle multiple user sessions.
    /// </summary>
    public sealed class IvsTrayMonitor
    {
        private readonly ILogger _logger = Log.ForContext<IvsTrayMonitor>();

        private static IvsTrayMonitor instance = null;

        private static readonly object padlock = new object();
        private readonly Timer timer = null;

        IvsTrayMonitor()
        {
            // Initial setup for the timer
            timer = new Timer
            {
                Interval = 1000 * 60 // fires every 60 second
            };

            timer.Elapsed += TimerElapsed;
        }

        public static IvsTrayMonitor Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new IvsTrayMonitor();
                    }
                    return instance;
                }
            }
        }

        public void StartMonitoring()
        {
            timer.Start();
        }

        public void StopMonitoring()
        {
            timer.Stop();

            var trayApps = Process.GetProcessesByName("IvsTray");
            if (trayApps.Count() > 0)
            {
                foreach (var item in trayApps)
                {
                    item.Kill();
                }
            }
        }

        private bool inTimer = false;

        private void TimerElapsed(object source, ElapsedEventArgs e)
        {
            if (inTimer)
            {
                return;
            }

            inTimer = true;

            try
            {
                //Check the user session is active or not
                var processes = Process.GetProcesses();
                bool isSessionActive = processes.Any(p => p.SessionId > 0 && p.ProcessName != "Idel");

                _logger.Verbose($"Is session active: {isSessionActive}");

                if (isSessionActive)
                {
                    //TODO: Handle closed event to start new tray app immediately.
                    Process trayApp = processes.FirstOrDefault(pp => pp.ProcessName.StartsWith("IvsTray"));

                    //TODO: Evaluate below scenario for multiple user sessions.
                    //Process myExplorer = Process.GetProcesses().FirstOrDefault(pp => pp.ProcessName == "explorer" && pp.SessionId == trayApp.SessionId);

                    if (trayApp != null)
                    {
                        _logger.Verbose($"Active Session App: {trayApp.ProcessName} - {trayApp.SessionId}");
                    }
                    else
                    {
                        var ivsTrayFile = CommonUtils.ConstructFromRoot("C:\\code\\invinsense-agent\\FormsTest\\bin\\Debug\\FormsTest.exe");
                        _logger.Information($"IvsTray is not running. Starting... {ivsTrayFile}");
                        ProcessExtensions.RunInActiveUserSession(null, ivsTrayFile);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while checking system tray");
            }

            inTimer = false;
        }
    }
}
