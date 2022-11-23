using System;
using System.ServiceProcess;

namespace IvsTray.Monitor
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
