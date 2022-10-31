using Common;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text.Json;

namespace IvsAgent
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "event_descriptions.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<TrackingEvent> items = JsonSerializer.Deserialize<List<TrackingEvent>>(json);
                var total = items.Count;
            }

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ivsagent.log"), rollingInterval: RollingInterval.Day)
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
                Log.Logger.Error(ex.StackTrace);
            }

            Log.CloseAndFlush();
        }
    }
}
