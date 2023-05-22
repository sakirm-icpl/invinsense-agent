﻿using Common.Utils;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using ToolManager;
using ToolManager.MsiWrapper;
using System.DirectoryServices.AccountManagement;
using ToolManager.AgentWrappers;

namespace IvsUninstall
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(CommonUtils.DataFolder + "\\IvsUninstall.log", rollOnFileSizeLimit: false, fileSizeLimitBytes: 100000)
               .WriteTo.Console()
               .CreateLogger();

                Log.Logger.Information("Uninstalling Invinsense 3.0 components");
                Log.Logger.Information("Finding the Invinsense service.....");

                var listPrograms = MoWrapper.ListPrograms();

                foreach (var item in listPrograms)
                {
                    //Log.Logger.Information($"Program: {item}");
                }

                Thread.Sleep(2000);

                string serviceName = "IvsAgent";

                ServiceController[] services = ServiceController.GetServices();
                ServiceController sc = services.FirstOrDefault(s => s.ServiceName == serviceName);
                if(sc != null) 
                { 
                    if(sc.Status==ServiceControllerStatus.Running) 
                    { 
                        sc.Stop();
                        Thread.Sleep(4000);
                    }
                }
                else
                {
                    Log.Logger.Information("The service has been stopped early..");
                }

                #region Deceptive Bytes - Endpoint Deception
                Log.Logger.Information("Uninstalling Deceptive Bytes...");

               if (DBytesWrapper.Verify(true)==0)
               {
                        var dBytesExitCode = ToolManager.AgentWrappers.DBytesWrapper.Remove();

                        Log.Logger.Information($"Deceptive Bytes remove exit code={dBytesExitCode}");

                        Thread.Sleep(3000);
               }
               else
               {
                    Log.Logger.Information("Deceptive Bytes alreay gets uninstalled");
               }
                
                #endregion

                #region OSQUERY

                Log.Logger.Information("Uninstalling OsQuery...");

                //Checking if file is exists or not
                if (OsQueryWrapper.Verify(true)==0)
                {
                        var osQueryExitCode = ToolManager.AgentWrappers.OsQueryWrapper.Remove();

                        Log.Logger.Information($"OSQUERY remove exit code={osQueryExitCode}");

                        Thread.Sleep(3000);
                }
                else
                {
                    Log.Logger.Information("Osquery already gets uninstalled");
                }

                //TODO:Removing folder : C:\Program Files\osquery
                var osqueryPath = "C:\\Program Files\\osquery";
                if (Directory.Exists(osqueryPath)) Directory.Delete(osqueryPath, true);

                Thread.Sleep(1000);

                #endregion

                #region WAZUH

                //Checking if file is exists or not
                if (WazuhWrapper.Verify(true)==0)
                {
                    Log.Logger.Information("Uninstalling Wazuh...");

                    var wazuhExitCode = ToolManager.AgentWrappers.WazuhWrapper.Remove();

                        Log.Logger.Information($"Wazuh remove exit code={wazuhExitCode}");

                        Thread.Sleep(3000);
                }
                else
                {
                    Log.Logger.Information("Wazuh alredy gets uninstalled");
                }
                //TODO:Removing folder : C:\\Program Files (x86)\\ossec
                var wazuhPath = "C:\\Program Files (x86)\\ossec";
                if (Directory.Exists(wazuhPath)) Directory.Delete(wazuhPath, true);

                Thread.Sleep(1000);

                #endregion

                #region SYSMON

                //Checking if file is exists or not
                if(SysmonWrapper.Verify(true)==0)
                {
                    Log.Logger.Information("Uninstalling Sysmon...");

                    var sysmonExitCode = ToolManager.AgentWrappers.SysmonWrapper.Remove();

                    Log.Logger.Information($"SYSMON remove exit code={sysmonExitCode}");

                    Thread.Sleep(3000);
                }
                else
                {
                    Log.Logger.Information("Sysmon alredy gets uninstalled");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }


            //Removing Agent with uninstall key
            try
            {
                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    Log.Logger.Information("MSI Installer is not free.");
                    return;
                }

                Log.Logger.Information("Agent Uninstallation is ready");

                var status = MsiPackageWrapper.Uninstall("Invinsense", "UNINSTALL_KEY=\"ICPL_2023\"");
                
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }

            //Unistalling all files from Infopercept folder from ProgramData Folder
            try
            {
                if (Directory.Exists(CommonUtils.DataFolder))
                {
                    DirectoryInfo directory = new DirectoryInfo(CommonUtils.DataFolder);
                    DateTime cutoffDate = DateTime.Now;
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        if (file.LastWriteTime < cutoffDate && file.Name!= "IvsUninstall.log" && file.Name!= "ivsuninstall.log")
                        {
                            file.Delete();
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }

            //Removing maintanence
            string username = "maintenance";
            using (PrincipalContext pc =new PrincipalContext(ContextType.Machine))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(pc,username);
                if (user != null) 
                {
                    Log.Logger.Information("Removing maintance user");
                    user.Delete();
                    Log.Logger.Information("Removing fake file");
                    var fakeFilePath = Path.Combine(@"C:\Users", "Users.txt");
                    if (File.Exists(fakeFilePath)) 
                    { 
                        File.Delete(fakeFilePath);
                    }
                }
            }
        }
    }
}
