using Serilog;
using System;
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
               .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Log.Logger.Information("Initializing program");

            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new SingleIvsAgent()
                };

                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Log.Logger.Error(ex.StackTrace);
            }

            Log.CloseAndFlush();
        }
    }
}
