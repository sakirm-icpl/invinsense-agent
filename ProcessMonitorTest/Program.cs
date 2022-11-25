using System;
using System.Diagnostics;
using System.Management;

namespace ProcessMonitorTest
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected Program() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main()
        {
            DateTime start = DateTime.Now;

            ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(StartWatchEventArrived);
            startWatch.Start();
            ManagementEventWatcher stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(StopWatchEventArrived);
            stopWatch.Start();
            Console.WriteLine("Press any key to exit");

            Process currentProcess = Process.GetCurrentProcess();

            while (!Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(1000);

                long usedMemory = currentProcess.PrivateMemorySize64;
                
                Console.WriteLine($"Used Memory: {usedMemory}, Processor Time: {currentProcess.TotalProcessorTime.TotalSeconds} / {(DateTime.Now-start).TotalSeconds}");
            }

            startWatch.Stop();
            stopWatch.Stop();
        }

        public static TimeSpan UpTime
        {
            get
            {
                using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();       //Call this an extra time before reading its value
                    return TimeSpan.FromSeconds(uptime.NextValue());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void StopWatchEventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

            if (processName.ToLower().Contains("notepad"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("Process stopped: {0}", processName);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void StartWatchEventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

            if (processName.ToLower().Contains("notepad"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine("Process started: {0}", e.NewEvent.Properties["ProcessName"].Value);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
