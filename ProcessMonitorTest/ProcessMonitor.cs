using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ProcessMonitorTest
{
    /// <summary>
    ///     https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id
    /// </summary>
    internal class ProcessMonitor
    {
        private static readonly PerformanceCounter perfCounter =
            new PerformanceCounter("Process", "% Processor Time", "msedge");

        //Private declarations.
        private readonly string processname;

        public ProcessMonitor()
        {
        }

        public ProcessMonitor(string program)
        {
            processname = program;
            Name = processname;
        }

        public string Name { get; set; }

        public bool IsProcessRunning()
        {
            var proc = Process.GetProcessesByName(processname);
            return !(proc.Length == 0 && proc == null);
        }

        public void Monitor()
        {
            Console.WriteLine("Monitoring {0} for high CPU usage...", processname);
            var intInterval = 10000; // 10 seconds

            while (IsProcessRunning())
            {
                Thread.Sleep(1000);


                Console.WriteLine("Process:{0} TCPU:{1} CPU% {2}", processname, GetTotalCpuTime(),
                    GetCurrentUsageIndividually());

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
                totalCpuTime += 100 * process.TotalProcessorTime.TotalSeconds /
                                (DateTime.Now - process.StartTime).TotalSeconds;

            return Math.Round(totalCpuTime / processes.Count(), 2);
        }


        public double GetCurrentUsage()
        {
            return Math.Round(perfCounter.NextValue(), 2);
        }

        public double GetCurrentUsageIndividually()
        {
            var watch = Stopwatch.StartNew();

            float totalCpu = 0;

            // grab all process instances
            var processes = Process.GetProcessesByName(processname);

            var processName = Path.GetFileNameWithoutExtension(processname);

            var cat = new PerformanceCounterCategory("Process");

            var instances = cat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            var processIds = processes.Select(x => x.Id).ToArray();

            foreach (var instance in instances)
                try
                {
                    using (var cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        var val = (int)cnt.RawValue;
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
                catch
                {
                }

            watch.Stop();

            Console.WriteLine($"Captured in {watch.ElapsedMilliseconds}");

            return totalCpu;
        }

        public void KillProcess()
        {
            var TaskKiller = "taskkill /f /im " + processname;

            try
            {
                var info = new ProcessStartInfo("cmd.exe", "/c " + TaskKiller)
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process
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
            var TaskKiller = "taskkill /f /im " + program;
            try
            {
                var info = new ProcessStartInfo("cmd.exe", "/c " + TaskKiller)
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process
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
        {
            Console.Read();
        }
    }
}