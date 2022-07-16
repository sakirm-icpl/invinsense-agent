using System;
using System.Diagnostics;
using System.Linq;

namespace EventLogSample
{
    internal class Program
    {
        static void Main()
        {
            WriteFromCustomEventSource();
            Console.ReadLine();
        }

        private static void Log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine("Event detected !");
            Console.WriteLine(e.Entry.Message);
        }

        private static void WriteFromCustomEventSource()
        {
            if(EventLog.GetEventLogs().Any(x=>x.LogDisplayName == "Invinsense"))
            {
                EventLog.CreateEventSource("SingleAgent", "Invinsense");
            }

            var log = new EventLog("Invinsense")
            {
                Source = "SingleAgent",
                EnableRaisingEvents = true
            };

            log.EntryWritten += Log_EntryWritten;

            log.WriteEntry("Test entry");
        }

        private static void WriteFromLogger()
        {
            Logger.EnsureEventSource();

            Logger.LogInformation("Test message");
        }
    }
}
