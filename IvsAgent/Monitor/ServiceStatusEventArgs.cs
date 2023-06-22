using System;
using System.ServiceProcess;

namespace IvsAgent.Monitor
{
    public class ServiceStatusEventArgs : EventArgs
    {
        public ServiceControllerStatus Status { get; private set; }

        public ServiceStatusEventArgs(ServiceControllerStatus status)
        {
            Status = status;
        }
    }
}
