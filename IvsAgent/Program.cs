using Common;
using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace IvsAgent
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Logging the ivsagent.json
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(CommonUtils.GetLogFilePath("IvsAgnt.log"), rollOnFileSizeLimit: true, retainedFileCountLimit:5, fileSizeLimitBytes: 30000, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Initializing service");

            try
            {
                //Check event log exists
                if (!EventLog.SourceExists(Constants.IvsAgentName))
                {
                    EventLog.CreateEventSource(Constants.IvsAgentName, Constants.LogGroupName);
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
