using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.Collections;
using Common;
using System.Diagnostics;

namespace Invinsense30
{

    [RunInstaller(true)]
    public class InvinsenseInstaller : Installer
    {
        private readonly ServiceInstaller _serviceInstaller;
        private readonly ServiceProcessInstaller _processInstaller;

        public InvinsenseInstaller()
        {
            // Instantiate installers for process and services.
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();

            // The services run under the system account.
            _processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            _serviceInstaller.StartType = ServiceStartMode.Automatic;

            // ServiceName must equal those on ServiceBase derived classes.
            _serviceInstaller.ServiceName = Constants.SingleAgentServiceName;
            _serviceInstaller.Description = "Single Agent Servcie";

            _serviceInstaller.AfterInstall += RunServiceAfterInstall;

            // Add installers to collection. Order is not important.
            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);
        }

        private void RunServiceAfterInstall(object sender, InstallEventArgs e)
        {
            System.Console.WriteLine("Running service");

            ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                //sc.Start();
            }

            EventLogHelper.EnsureEventSource(Constants.SingleAgentLogSourceName);

            EventLogHelper.AddEvent(EventLogEntryType.Information, Constants.SingleAgentLogSourceName, EventId.AvDisabled);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            //EventLogHelper.EnsureEventSource("SingleAgent");
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);

          //  EventLog.DeleteEventSource("SingleAgent");
        }
    }
}