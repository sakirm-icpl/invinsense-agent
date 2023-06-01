﻿using System.ServiceProcess;

namespace UserSessionAppMonitor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AppMonitorService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
