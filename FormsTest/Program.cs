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
        /// https://devforid.medium.com/localization-in-c-using-resource-resx-file-a90117f04d4a
        /// </summary>
        [STAThread]
        static void Main()
        {
            var cultureValue = Environment.GetEnvironmentVariable("IVS_CULTURE", EnvironmentVariableTarget.Machine);
            var culture = new CultureInfo(string.IsNullOrEmpty(cultureValue) ? "en-US" : cultureValue);

            Debug.WriteLine("CurrentCulture: " + culture);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new MainFrm());
        }
    }
}
