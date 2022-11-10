using System.Diagnostics;
using System;
using ServiceMonitorTest.Monitor;
using System.ServiceProcess;
using ServiceMonitorTest.Helper;
using System.Linq;

namespace ServiceMonitorTest
{
    internal class Program
    {
        static void Main()
        {
            ExtendedServiceController serviceControl;

            serviceControl = new ExtendedServiceController("SampleService");
            serviceControl.StatusChanged += (object sender, ServiceStatusEventArgs e) => ServiceUpdateStatus(e.Status);

            Console.WriteLine("Ready to take command");

            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                if (string.IsNullOrEmpty(line))
                {
                    var currentProcess = Process.GetCurrentProcess();
                    var msg = $"ID: {currentProcess.Id}, Name: {currentProcess.ProcessName} on {currentProcess.MachineName} CPU(T): {currentProcess.TotalProcessorTime} CPU(U):{currentProcess.UserProcessorTime}";
                    Console.WriteLine(msg);
                    continue;
                }

                if (line.Equals("r"))
                {
                    PerformanceCounter cpuCounter;
                    PerformanceCounter ramCounter;

                    cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    ramCounter = new PerformanceCounter("Memory", "Available MBytes");


                    Console.WriteLine($"CPU: {cpuCounter.NextValue()}% RAM: {ramCounter.NextValue()}MB");
                }

                var tokens = line.Split(' ');

                if (line.Equals("s"))
                {
                    Console.WriteLine($"CurrentStatus: {serviceControl.Status}");
                }

                if(line.Equals("l"))
                {
                    serviceControl.StartListening();
                    Console.WriteLine("Started listening");
                }

                if(line.Equals("c"))
                {
                    serviceControl.Dispose();

                    serviceControl = new ExtendedServiceController("SampleService");
                    serviceControl.StatusChanged += (object sender, ServiceStatusEventArgs e) => ServiceUpdateStatus(e.Status);
                }

                if(line.Equals("i"))
                {
                    string[] args = new string[] { };
                    ServiceHelper.InstallAndStart("SampleService", "My Sample Service", "C:\\Code\\invinsense-agent\\SampleService\\bin\\Debug\\SampleService.exe", args);
                }

                if (line.Equals("u"))
                {
                    ServiceHelper.Uninstall("SampleService");
                }
            }
        }

        private static void ServiceUpdateStatus(ServiceControllerStatus? status)
        {
            Console.WriteLine($"Status: {status?.ToString()}");
        }
    }
}
