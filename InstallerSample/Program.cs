using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

                logger.Info("Removing Service");
                var exePath ="C:\\Windows\\System32\\sc.exe";
                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                         FileName = exePath,
                         Arguments = "delete \"Invinsense Agent\"",
                         WindowStyle = ProcessWindowStyle.Hidden,
                         CreateNoWindow = true
                     }
                 };

                installerProcess.Start();
                logger.Info("Remove Service Successfully");
                
                logger.Info("Uninstalling Invinsense Agent");

                var unInstallStatus = UninstallProgram("Invinsense Single Agent 3.0");

                logger.Info($"Agent uninstall status: {unInstallStatus}");

               // logger.Info($"Removing Startup");

               // SetStartup("IvsTray", "C:\\ProgramFiles\\Invinsense\\IvsTray.exe", false);

               // logger.Info("Startup removed");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                //logger.Error(ex.StackTrace);
            }
            finally
            {
                Logger.ShutDown(); // Removing this line may result in lost log entries.
            }

            Console.WriteLine("Done.");
        }     

        public static bool UninstallProgram(string ProgramName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");

                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            object hr = mo.InvokeMethod("Uninstall", null);

                            Console.WriteLine($"Uninstall invoke return code: {hr}");

                            return (bool)hr;
                        }
                    }
                    catch
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }

                //was not found...
                return false;

            }
            catch
            {
                return false;
            }
        }
        void UnInstallPackage(string packageName)
        {
            string searchString = $"SELECT * FROM Win32_Product WHERE Name LIKE '{packageName}'";

            ManagementObjectSearcher mos = new ManagementObjectSearcher(searchString);

            foreach (ManagementObject mo in mos.Get())
            {
                // Will return Name, IdentifyingNumber and Version
                try
                {
                    Console.WriteLine("Properties");
                    foreach (var item in mo.Properties)
                    {
                        try
                        {
                            Console.WriteLine(item.Name + "-" + item.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in property: {ex.Message}");
                        }
                    }

                    Console.WriteLine("System Properties");
                    foreach (var item in mo.SystemProperties)
                    {
                        try
                        {
                            Console.WriteLine(item.Name + "-" + item.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in property: {ex.Message}");
                        }
                    }

                    if (mo["Name"].ToString().Contains(packageName))
                    {
                        //object arp = mo.InvokeMethod("Uninstall", null);
                        Process process = new Process();
                        process.StartInfo.FileName = "msiexec.exe";
                        process.StartInfo.Arguments = "/X" + mo["IdentifyingNumber"].ToString() + " /q";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = false;
                        process.Start();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    //this program may not have a name property, so an exception will be thrown
                    //handle your exception here
                    Console.WriteLine(ex.Message);
                }            
            }
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
