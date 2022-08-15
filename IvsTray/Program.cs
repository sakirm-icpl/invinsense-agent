using Serilog;
using System;
using System.IO;
using System.Windows.Forms;

namespace IvsTray
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
               .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Application.Run(new MainForm());
        }
    }
}
