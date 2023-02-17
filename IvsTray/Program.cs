﻿using Common.Utils;
using Serilog;
using System;
using System.Threading;
using System.Windows.Forms;
using Serilog.Formatting.Json;
using Microsoft.Win32;
using System.ServiceProcess;
using FluentAssertions.Common;

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
               .WriteTo.File(new JsonFormatter(),CommonUtils.DataFolder + "\\IvsTray.json", rollOnFileSizeLimit: false, fileSizeLimitBytes: 100000)
               .CreateLogger();

            Log.Logger.Information("Initializing program");
                
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
