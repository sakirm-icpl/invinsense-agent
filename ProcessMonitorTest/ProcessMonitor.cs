using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ProcessMonitorTest
{
    /// <summary>
    /// https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id
    /// </summary>
    internal class ProcessMonitor
    {
        //Private declarations.
        private string processname;

        public ProcessMonitor()
        {

        }

        public ProcessMonitor(string program)
        {
            processname = program;
            Name = processname;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsProcessRunning()
        {
            Process[] proc = Process.GetProcessesByName(processname);
            return !(proc.Length == 0 && proc == null);
        }

        public void Monitor()
        {
            Console.WriteLine("Monitoring {0} for high CPU usage...", processname);
            int intInterval = 10000; // 10 seconds

            while (IsProcessRunning() == true)
            {
                Thread.Sleep(1000);


                Console.WriteLine("Process:{0} TCPU:{1} CPU% {2}", processname, GetTotalCpuTime(), GetCurrentUsageIndividually());

                /*
                   if (pcProcess.NextValue() > float.Parse("10") ? true : false)
                   {
                       Console.WriteLine(string.Format("Killing {0} {1} at {2}", process.Id, processname, DateTime.Now.ToString()));
                       KillProcess(processname); //Kills the running process.
                   }
                   */

                Console.WriteLine("========================");

                // Sleep till the next loop
                Thread.Sleep(intInterval);
            }
        }

        public double GetTotalCpuTime()
        {
            // grab all process instances
            var processes = Process.GetProcessesByName(processname);
            double totalCpuTime = 0;

            foreach (var process in processes)
            {
                totalCpuTime += 100 * process.TotalProcessorTime.TotalSeconds / (DateTime.Now - process.StartTime).TotalSeconds;
            }

            return Math.Round(totalCpuTime / processes.Count(), 2);
        }

        private static PerformanceCounter perfCounter = new PerformanceCounter("Process", "% Processor Time", "msedge");


        public double GetCurrentUsage()
        {
            return Math.Round(perfCounter.NextValue() , 2);
        }

        public double GetCurrentUsageIndividually()
        {
            var watch = Stopwatch.StartNew();

            float totalCpu = 0;

            // grab all process instances
            var processes = Process.GetProcessesByName(processname);

            string processName = Path.GetFileNameWithoutExtension(processname);

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            var instances = cat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            var processIds = processes.Select(x=>x.Id).ToArray();

            foreach (string instance in instances)
            {
                try
                {
                    using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        int val = (int)cnt.RawValue;
                        if (processIds.Contains(val))
                        {
                            var counter = new PerformanceCounter("Process", "% Processor Time", instance);

                            // start capturing
                            counter.NextValue();

                            var value = counter.NextValue() / Environment.ProcessorCount;

                            totalCpu += value;

                            Console.WriteLine($"{instance} - {value}");
                        }
                    }
                }
                catch { }
                
            }

            watch.Stop();

            Console.WriteLine($"Captured in {watch.ElapsedMilliseconds}");

            return totalCpu;
        }

        public void KillProcess()
        {
            string TaskKiller = "taskkill /f /im " + processname;

            try
            {
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + TaskKiller);
                info.RedirectStandardError = true;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                Process process = new Process
                {
                    StartInfo = info
                };

                process.Start();
                process.StandardOutput.ReadToEnd();
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
        }

        public void KillProcess(string program)
        {
            string TaskKiller = "taskkill /f /im " + program;
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + TaskKiller);
                info.RedirectStandardError = true;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                Process process = new Process
                {
                    StartInfo = info
                };

                process.Start();
                process.StandardOutput.ReadToEnd();
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
        }

        public void FreezeOnScreen()
        { Console.Read(); }
    }
}
