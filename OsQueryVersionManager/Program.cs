using System;
using System.IO;

namespace OsQueryChecker
{
    internal class Program
    {
        private static string MonitoringPath = @"C:\ProgramData\Infopercept\Artifacts";
        private static Version MaxVersion = new Version("5.8.2"); // Set your required version

        static void Main(string[] args)
        {
            if(!Directory.Exists(@"C:\ProgramData\Infopercept"))
            {
                Console.WriteLine("Creating directory C:\\ProgramData\\Infopercept");
                Directory.CreateDirectory(@"C:\ProgramData\Infopercept");
            }

            if(!Directory.Exists(@"C:\ProgramData\Infopercept\Artifacts"))
            {
                Console.WriteLine("Creating directory C:\\ProgramData\\Infopercept\\Artifacts");
                Directory.CreateDirectory(@"C:\ProgramData\Infopercept\Artifacts");
            }

            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = MonitoringPath;
                watcher.Filter = "osquery*";
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                watcher.Created += OnNewFileDetected;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Monitoring for new osquery files at: " + MonitoringPath);
                Console.ReadLine(); // Keep the application running
            }
        }

        private static void OnNewFileDetected(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"New file detected: {e.FullPath}");

            // Implement version checking and updating logic here
            var installedVersion = tooln.GetInstalledVersion();

            if (installedVersion == null || installedVersion <= MaxVersion)
            {
                Console.WriteLine("Upgrade is required");
            }
            else
            {
                Console.WriteLine("Skipping upgrade");
            }
        }
    }
}
