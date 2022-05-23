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

            //Initializing logger with local data
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiteDbAsync(@"c:\tmp\logs.db", retentionPeriod: new TimeSpan(7, 0, 0, 0))
                .CreateLogger();

            Application.Run(new MainForm());
        }
    }
}
