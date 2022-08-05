using Serilog;
using System;
using System.Windows.Forms;

namespace SingleAgent
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Application.Run(new MainForm());
        }
    }
}
