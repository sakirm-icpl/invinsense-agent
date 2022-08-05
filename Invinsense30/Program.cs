using Serilog;
using System.ServiceProcess;

namespace Invinsense30
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SingleAgentService()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
