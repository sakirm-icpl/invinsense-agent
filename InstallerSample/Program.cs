using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace InstallerSample
{
    internal class Program
    {
        static void Main()
        {
            var targetLogFile = new FileInfo("./uninstall.log");
            Logger.LogToConsole = true;  // Print log entries to console (optional).
            Logger.Start(targetLogFile); // Loggers will complains if you skip initialization

            var logger = new Logger(typeof(Program));

            try
            {

             /*   List<string> programs = new List<string>();

                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        var name = mo["Name"].ToString();
                        programs.Add(name);
                        logger.Info($"Program: {name}");
                    }
                    catch
                    {
                        //this program may not have a name property
                    }
                }*/


                logger.Info("Stopping Invinsense service");

                var service = new ServiceController("Invinsense Agent");
                service.Stop();

                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));

                logger.Info("Service is stopped");

               // Console.ReadLine();

                /*
                foreach (var process in Process.GetProcesses())
                {
                    logger.Info($"Process: {process.Id}, Name: {process.ProcessName}");
                }
                */

                logger.Info("Trying to stop IvsTray");

                foreach (var process in Process.GetProcessesByName("IvsTray"))
                {
                    logger.Info($"Stopping: {process.Id}");
                    process.Kill();
                }

               // Console.ReadLine();

                logger.Info("Uninstalling Invinsense Agent");

                var unInstallStatus = UninstallWrapper.UninstallProgram("Invinsense Single Agent 3.0");

                logger.Info($"Agent uninstall status: {unInstallStatus}");

               // logger.Info($"Removing Startup");

               // SetStartup("IvsTray", "C:\\ProgramFiles\\Invinsense\\IvsTray.exe", false);

               // logger.Info("Startup removed");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                Logger.ShutDown(); // Removing this line may result in lost log entries.
            }

            Console.WriteLine("Done.");

           // Console.ReadLine();

        }


        private static void SetStartup(string name, string path, bool addOrRemove)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (addOrRemove)
                rk.SetValue(name, path);
            else
                rk.DeleteValue(name, false);
        }
    }
}
