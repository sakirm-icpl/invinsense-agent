using Common;
using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
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
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(CommonUtils.GetAbsoletePath("ivsagent.log"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Log.Logger.Information("Initializing service");

            try
            {
                //Check event log exists
                if (!EventLog.SourceExists(Common.Constants.IvsAgentName))
                {
                    EventLog.CreateEventSource(Common.Constants.IvsAgentName, Common.Constants.LogGroupName);
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
