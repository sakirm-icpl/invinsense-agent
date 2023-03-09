using System;
using System.Diagnostics;
using System.Threading;

namespace ProcessMonitorTest
{
    /// <summary>
    /// https://michaelscodingspot.com/performance-counters/
    /// https://michaelscodingspot.com/find-fix-and-avoid-memory-leaks-in-c-net-8-best-practices/
    /// </summary>
    public class PerfMonSample
    {
        /// <summary>
        /// 
        /// Use the Process.GetProcessesByName method in C# to get a list of all the processes that match the name of the application you want to monitor.
        /// Use the PerformanceCounter class in C# to create performance counters for each process you want to monitor. 
        /// You can create counters for CPU usage (Processor\% Processor Time), working set memory usage (Process\Working Set), and private memory usage (Process\Private Bytes).
        /// Use the PerformanceCounter.NextValue method in a loop to continuously capture the values of the performance counters at regular intervals. 
        /// You can specify the sampling interval using the Thread.Sleep method.
        /// Use the captured performance counter values to analyze the CPU and memory usage of the application over time.
        /// </summary>
        /// <param name="args"></param>
        public static void Monitor(params string[] args)
        {
            // Get a list of all the processes that match the name of the application you want to monitor
            Process[] processes = Process.GetProcessesByName(args[0]);

            // Create performance counters for each process you want to monitor
            PerformanceCounter[] cpuCounters = new PerformanceCounter[processes.Length];
            PerformanceCounter[] workingSetCounters = new PerformanceCounter[processes.Length];
            PerformanceCounter[] privateBytesCounters = new PerformanceCounter[processes.Length];

            for (int i = 0; i < processes.Length; i++)
            {
                cpuCounters[i] = new PerformanceCounter("Processor", "% Processor Time", processes[i].ProcessName, true);
                workingSetCounters[i] = new PerformanceCounter("Process", "Working Set", processes[i].ProcessName, true);
                privateBytesCounters[i] = new PerformanceCounter("Process", "Private Bytes", processes[i].ProcessName, true);
            }

            // Continuously capture the values of the performance counters at regular intervals
            while (true)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    float cpuUsage = cpuCounters[i].NextValue();
                    long workingSet = workingSetCounters[i].NextSample().RawValue;
                    long privateBytes = privateBytesCounters[i].NextSample().RawValue;

                    // Do something with the captured performance counter values
                    // For example, you could write them to a log file
                    Console.WriteLine("CPU usage: {0}%", cpuUsage);
                    Console.WriteLine("Working set: {0} bytes", workingSet);
                    Console.WriteLine("Private bytes: {0} bytes", privateBytes);
                }

                Thread.Sleep(1000); // Wait for 1 second before capturing the next set of values
            }
        }
    }
}