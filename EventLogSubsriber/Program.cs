using Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace EventLogSubscriber
{
    /// <summary>
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        protected Program()
        {
        }

        /// <summary>
        /// </summary>
        private static void Main()
        {
            var log = new EventLog(Constants.LogGroupName)
            {
                EnableRaisingEvents = true
            };

            log.EntryWritten += Log_EntryWritten;

            Console.WriteLine("Press any key to exit");
            while (!Console.KeyAvailable) Thread.Sleep(50);

            log.EntryWritten -= Log_EntryWritten;

            Console.WriteLine("Application is closed");
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine("Event detected ! - " + e.Entry.Source + " " + e.Entry.Message);
        }
    }
}