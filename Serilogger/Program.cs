using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Threading;

namespace Serilogger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
            Log.Information("No one listens to me!");

            // Finally, once just before the application exits...
            Log.CloseAndFlush();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Ah, there you are!");

            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.File(new CustomLogFormatter(), "logs.json", rollOnFileSizeLimit: false, restrictedToMinimumLevel: LogEventLevel.Information, fileSizeLimitBytes: 10000)
                   .CreateLogger();

            Log.Information("Ah, there you are!");

            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Ah, there you are!");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 500, retainedFileCountLimit: 1, rollingInterval: RollingInterval.Infinite)
                .CreateLogger();

            Log.Information("Initializing service");

            while (true)
            {
                Log.Information(DateTime.Now.ToString());
                Thread.Sleep(200);
            }
        }
    }

    class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}
