using Common;
using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using Serilog.Formatting.Json;
using System.IO;

namespace IvsAgent
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Uninstalling the files before installing the agent
            try
            {
                if (Directory.Exists(CommonUtils.DataFolder))
                {
                    Log.Logger.Information("Removing files from ProgramData/Infopercept");
                    DirectoryInfo directory=new DirectoryInfo(CommonUtils.DataFolder);
                    DateTime cutoffDate=DateTime.Now;
                    foreach(FileInfo file in directory.GetFiles()) 
                    { 
                        if(file.LastWriteTime<cutoffDate)
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch { }

            //Logging the ivsagent.json
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
           .WriteTo.File(
                new JsonFormatter(),
                CommonUtils.DataFolder + "\\ivsagent.json",
                rollOnFileSizeLimit: false,
                fileSizeLimitBytes: 10000)
           .CreateLogger();

            Log.Information("Initializing service");

            try
            {
                //Check event log exists
                if (!EventLog.SourceExists(Constants.IvsAgentName))
                {
                    EventLog.CreateEventSource(Constants.IvsAgentName, Common.Constants.LogGroupName);
                }

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new IvsService()
                };

                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            Log.CloseAndFlush();
        }
    }
}
