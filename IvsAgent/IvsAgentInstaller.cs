using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using Common;
using System.Diagnostics;
using System;
using IvsAgent.AgentWrappers;
using Serilog;
using System.IO;

namespace IvsAgent
{

    [RunInstaller(true)]
    public class IvsAgentInstaller : Installer
    {
        private readonly ServiceInstaller _serviceInstaller;
        private readonly ServiceProcessInstaller _processInstaller;

        private readonly EventLogInstaller _eventLogInstaller;

        private readonly ILogger _logger;

        public IvsAgentInstaller()
        {

            _logger = new LoggerConfiguration()
              .MinimumLevel.Verbose()
              .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ivsinstaller.log"), rollingInterval: RollingInterval.Day)
              .CreateLogger();

            // Create an instance of an EventLogInstaller.
            _eventLogInstaller = new EventLogInstaller
            {
                // Set the source name of the event log.
                Source = Constants.IvsAgentName,

                // Set the event log that the source writes entries to.
                Log = Constants.LogGroupName
            };

            // Add myEventLogInstaller to the Installer collection.
            Installers.Add(_eventLogInstaller);


            // Instantiate installers for process and services.
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();

            // The services run under the system account.
            _processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            _serviceInstaller.StartType = ServiceStartMode.Automatic;

            // ServiceName must equal those on ServiceBase derived classes.
            _serviceInstaller.ServiceName = Constants.IvsName;
            _serviceInstaller.Description = Constants.IvsDescription;

            _serviceInstaller.AfterInstall += RunServiceAfterInstall;

            _serviceInstaller.BeforeUninstall += RemoveComponentsBeforeUninstall;

            // Add installers to collection. Order is not important.
            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);

            _logger.Information($"CTOR{Environment.NewLine}");
        }

        private void RunServiceAfterInstall(object sender, InstallEventArgs e)
        {
            base.OnAfterInstall(e.SavedState);

            _logger.Information($"Start service{Environment.NewLine}");
            try
            {
                foreach (System.Collections.DictionaryEntry item in Context.Parameters)
                {
                    _logger.Information($"Key: {item.Key}, Value: {item.Value} {Environment.NewLine}");
                }

                ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
                using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
                {
                    sc.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }
        }

        private void RemoveComponentsBeforeUninstall(object sender, InstallEventArgs e)
        {
            _logger.Information($"RemoveComponents{Environment.NewLine}");
          
            try
            {
                using (ServiceController sv = new ServiceController(Constants.IvsName))
                {
                    if (sv.Status != ServiceControllerStatus.Stopped)
                    {
                        _logger.Error("Stopping service");

                        sv.Stop();
                        sv.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Service failed to stop: {ex.Message}");
            }

            System.Threading.Thread.Sleep(2000);

            var ivsTrayProcess = Process.GetProcessesByName("IvsTray");
            if (ivsTrayProcess.Length > 0)
            {
                _logger.Information("Stopping IvsTray app");
                ivsTrayProcess[0].Kill();
            }

            try
            {
                var osQueryRemovalReturnCode = OsQueryWrapper.Remove();

                _logger.Information($"OSQuery removal exit code: {osQueryRemovalReturnCode}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }

            base.OnBeforeUninstall(e.SavedState);
        }
    }
}