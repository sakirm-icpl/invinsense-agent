using System;
using System.Diagnostics;

namespace EventLogSubscriber
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected Program() { }

        /// <summary>
        /// 
        /// </summary>
        static void Main()
        {
            var log = new EventLog(Common.Constants.LogGroupName)
            {
                EnableRaisingEvents = true
            };

            log.EntryWritten += Log_EntryWritten;

            Console.WriteLine("Press any key to exit");
            while (!Console.KeyAvailable) System.Threading.Thread.Sleep(50);

            log.EntryWritten -= Log_EntryWritten;

            Console.WriteLine("Application is closed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine("Event detected ! - " + e.Entry.Source + " " + e.Entry.Message);
        }
    }
}
