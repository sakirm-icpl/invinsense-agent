using System;
using System.ServiceProcess;

namespace ServiceMonitorTest.Monitor
{
    public class ServiceStatusEventArgs : EventArgs
    {
        public ServiceStatusEventArgs(ServiceControllerStatus status)
        {
            Status = status;
        }

        public ServiceControllerStatus Status { get; private set; }
    }
}