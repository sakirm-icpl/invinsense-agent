using System;
using System.ServiceProcess;

namespace Common.ServiceHelpers
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
