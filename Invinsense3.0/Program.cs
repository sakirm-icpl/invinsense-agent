using System.ServiceProcess;

namespace Invinsense3._0
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SingleAgentService()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
