using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace FormsTest
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Debug.WriteLine("CurrentCulture: " + CultureInfo.CurrentCulture);
            Debug.WriteLine("CurrentUICulture: " + CultureInfo.CurrentUICulture);


            // Set the current thread's culture to the OS culture
            //Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("hi-IN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("hi-IN");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new MainFrm());
        }
    }
}
