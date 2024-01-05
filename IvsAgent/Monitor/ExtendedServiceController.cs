using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using ToolManager.Models;

namespace IvsAgent.Monitor
{
    public class ExtendedServiceController : ServiceController
    {
        public event EventHandler<ServiceStatusEventArgs> StatusChanged;
        private readonly Dictionary<ServiceControllerStatus, Task> _tasks = new Dictionary<ServiceControllerStatus, Task>();

        new public ServiceControllerStatus? Status
        {
            get
            {
                Refresh();
                try
                {
                    return base.Status;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public InstallStatus InstallStatus
        {
            get
            {
                return Status == null ? InstallStatus.NotFound : InstallStatus.Installed;
            }
        }

        public RunningStatus RunningStatus
        {
            get
            {
                if(Status == null)
                {
                    return RunningStatus.Error;
                }

                switch (Status)
                {
                    case ServiceControllerStatus.Running:
                        return RunningStatus.Running;
                    case ServiceControllerStatus.Stopped:
                        return RunningStatus.Stopped;
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.StopPending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.PausePending:
                        return RunningStatus.Warning;
                    default:
                        return RunningStatus.Warning;
                }
            }
        }

        public ExtendedServiceController(string ServiceName) : base(ServiceName)
        {
            foreach (ServiceControllerStatus status in Enum.GetValues(typeof(ServiceControllerStatus)))
            {
                _tasks.Add(status, null);
            }
        }

        public void StartListening()
        {
            if(Status == null)
            {
                return;
            }

            foreach (ServiceControllerStatus status in Enum.GetValues(typeof(ServiceControllerStatus)))
            {
                if (Status != status && (_tasks[status] == null || _tasks[status].IsCompleted))
                {
                    _tasks[status] = Task.Run(() =>
                    {
                        try
                        {
                            WaitForStatus(status);
                            OnStatusChanged(new ServiceStatusEventArgs(status));
                            StartListening();
                        }
                        catch
                        {
                            // You can either raise another event here with the exception or ignore it
                            // since it most likely means the service was uninstalled/lost communication
                        }
                    });
                }
            }
        }

        protected virtual void OnStatusChanged(ServiceStatusEventArgs e)
        {
            EventHandler<ServiceStatusEventArgs> handler = StatusChanged;
            handler?.Invoke(this, e);
        }
    }
}
