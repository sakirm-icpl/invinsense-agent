using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using Common;
using System.Diagnostics;
using System;

namespace IvsAgent
{

    [RunInstaller(true)]
    public class IvsAgentInstaller : Installer
    {
        private readonly ServiceInstaller _serviceInstaller;
        private readonly ServiceProcessInstaller _processInstaller;

        private readonly EventLogInstaller _eventLogInstaller;

        public IvsAgentInstaller()
        {
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
            _serviceInstaller.ServiceName = "Invinsense";
            _serviceInstaller.Description = "Invinsense 3.0";

            _serviceInstaller.AfterInstall += RunServiceAfterInstall;

            // Add installers to collection. Order is not important.
            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);
        }

        private void RunServiceAfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                foreach (System.Collections.DictionaryEntry item in Context.Parameters)
                {
                    Context.LogMessage($"Key: {item.Key}, Value: {item.Value} {Environment.NewLine}");
                }

                ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
                using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
                {
                    sc.Start();
                }
            }
            catch (Exception ex)
            {
                Context.LogMessage(ex.StackTrace);
            }
        }
    }
}