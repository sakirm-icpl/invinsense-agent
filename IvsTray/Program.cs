using Common.Utils;
using Serilog;
using System;
using System.Threading;
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
               .WriteTo.File(CommonUtils.GetAbsoletePath("IvsTray.log"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Log.Logger.Information("Initializing program");

            Common.Persistance.SeedClass.SeedData();

            AppDomain.CurrentDomain.UnhandledException += GlobalHandler;

            Application.ThreadException += ApplicationThreadException;

            Application.Run(new MainForm());
        }

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;

            string LF = Environment.NewLine + Environment.NewLine;
            string infos = $"Message : {LF}{ex.Message}{LF}" +
                           $"Source : {LF}{ex.Source}{LF}" +
                           $"Stack : {LF}{ex.StackTrace}{LF}" +
                           $"InnerException : {ex.InnerException}";

            Log.Logger.Error(infos);
        }

        static void GlobalHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            string LF = Environment.NewLine + Environment.NewLine;
            string infos = $"Message : {LF}{ex.Message}{LF}" +
                           $"Source : {LF}{ex.Source}{LF}" +
                           $"Stack : {LF}{ex.StackTrace}{LF}" +
                           $"InnerException : {ex.InnerException}";

            Log.Logger.Error(infos);
        }
    }
}
