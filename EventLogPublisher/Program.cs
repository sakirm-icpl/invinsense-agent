using System;
using System.Diagnostics;
using System.Linq;

namespace EventLogSample
{
    public static class Program
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
            Console.WriteLine("enter event name for publishing");
            String getevent = Console.ReadLine();
            EventLog eventLog = new EventLog();
            eventLog.Source = "SingleAgent";
            eventLog.WriteEntry(getevent, EventLogEntryType.Information,1010,1);

            if (EventLog.GetEventLogs().Any(x=>x.LogDisplayName == getevent))
            {
                EventLog.CreateEventSource("SingleAgent", getevent);
            }

            var log = new EventLog()
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
