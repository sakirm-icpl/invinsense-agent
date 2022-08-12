using Serilog;

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

            Log.Logger.Information("Initializing program");

            try
            {
#if (!DEBUG)

            System.Console.WriteLine("Starting service");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SingleAgentService()
            };

            ServiceBase.Run(ServicesToRun);
#else
                var serviceCall = new SingleAgentService();

                System.Console.ReadLine();
#endif

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
                Log.Logger.Error(ex.StackTrace);
            }

            Log.CloseAndFlush();
        }
    }
}
