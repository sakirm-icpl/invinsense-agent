using System;
using System.Diagnostics;

namespace EventLogPublisher
{
    /// <summary>
    /// Powershell Commands:
    ///     Remove-EventLog -LogName "Invinsense"
    ///     
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// default constructor
        /// </summary>
        protected Program() { }

        static void Main()
        {
            Console.WriteLine("Press \"q\" to stop");

            string logName = "Infopercept";
            string sourceName = "SingleAgent";

            string str;
            while ((str = Console.ReadLine()) != "q")
            {
                if(str.StartsWith("create"))
                {
                    EventLog.CreateEventSource(sourceName, logName);
                    continue;
                }

                if(str.StartsWith("delete"))
                {
                    if (EventLog.SourceExists(sourceName))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(sourceName, ".")}");
                        EventLog.DeleteEventSource(sourceName);
                    }
                    else
                    {
                        Console.WriteLine("Source not exists");
                    }
                    
                    if (EventLog.Exists(logName))
                    {
                        Console.WriteLine($"Log exists {logName}");
                        EventLog.Delete(logName);
                    }
                    else
                    {
                        Console.WriteLine("Log not exists");
                    }

                    continue;
                }

                if (str.StartsWith("check"))
                {
                    if(EventLog.Exists(logName))
                    {
                        Console.WriteLine("Log exists");
                    }
                    else
                    {
                        Console.WriteLine("Log not exists");
                    }

                    if(EventLog.SourceExists(sourceName))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(sourceName, ".")}");
                    }
                    else
                    {
                        Console.WriteLine("Source not exists");
                    }

                    continue;
                }

                if(str.StartsWith("entry"))
                {
                    EventLog.WriteEntry(sourceName, "Test message");
                }

                if(str.StartsWith("entry2"))
                {
                    var log = new EventLog(logName)
                    {
                        Source = sourceName
                    };

                    log.WriteEntry("Test entry", EventLogEntryType.Information);
                }
            }
        }
    }
}
