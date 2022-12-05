using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitoring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var process in Process.GetProcesses())
            {
                Process[] localAll = Process.GetProcesses();
                Console.WriteLine($"Process: {process.Id}, Name: {process.ProcessName}");
            }
            Console.ReadLine();
            // Process p= msedge; /*get the desired process here*/
            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", "msedge");
            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", "msedge");
            while (true)
            {
                Thread.Sleep(500);
                double ram = ramCounter.NextValue();
                double cpu = cpuCounter.NextValue();
                Console.WriteLine("Microsoft edge RAM: " + (ram / 1024 / 1024) + " MB; CPU: " + (cpu));
            }
        }
    }
}