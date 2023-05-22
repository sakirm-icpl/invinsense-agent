using Common;
using Common.Utils;
using Serilog;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using Serilog.Formatting.Json;

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
                .MinimumLevel.Debug()
                .WriteTo.File(new JsonFormatter(), CommonUtils.DataFolder + "\\ivsagent.json", rollOnFileSizeLimit: false, fileSizeLimitBytes: 30000)
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
