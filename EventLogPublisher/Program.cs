using Common;
using ConsoleMenu;
using System;
using System.Diagnostics;

namespace EventLogPublisher
{
    /// <summary>
    ///     PowerShell Commands:
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            var consoleMenu = new ConsoleMenuUtility();
            consoleMenu.DisplayMenuAndHandleInput();
        }

        [ConsoleOption(1, "Create Event Log Source")]
        public static void CreateEventLogSource()
        {
            if (EventLog.SourceExists(Constants.IvsAgentName))
            {
                Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");
                EventLog.WriteEntry(Constants.IvsAgentName, "Test message", EventLogEntryType.Warning);                
            }
            else
            {
                Console.WriteLine($"Creating {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");
                EventLog.CreateEventSource(Constants.IvsAgentName, Constants.CompanyName);
            }
        }

        /// <summary>
        /// An event log source should not be created and immediately used.
        /// There is a latency time to enable the source, it should be created
        /// prior to executing the application that uses the source.
        /// Execute this sample a second time to use the new source.
        /// </summary>
        [ConsoleOption(2, "Write Entry")]
        public static void CreateEventSource()
        {
            var log = new EventLog
            {
                Source = Constants.IvsAgentName
            };

            log.WriteEntry("Test entry", EventLogEntryType.Information);
        }

        [ConsoleOption(3, "Delete Event Log Source")]
        public static void DeleteEventLogSource()
        {
            if (EventLog.SourceExists(Constants.IvsAgentName))
            {
                Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");
                EventLog.DeleteEventSource(Constants.IvsAgentName);
            }
            else
            {
                Console.WriteLine("Source not exists");
            }

            if (EventLog.Exists(Constants.CompanyName))
            {
                Console.WriteLine($"Log exists {Constants.CompanyName}");
                EventLog.Delete(Constants.CompanyName);
            }
            else
            {
                Console.WriteLine("Log not exists");
            }
        }

        [ConsoleOption(4, "Write MySource")]
        public static void TestMyLogSource()
        {
            if (EventLog.SourceExists("MySource"))
            {
                Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName("MySource", ".")}");
                EventLog.DeleteEventSource("MySource");
                EventLog.Delete("MyNewLog");
                return;
            }

            Console.WriteLine("Source not exists. creating...");
            EventLog.CreateEventSource("MySource", "MyNewLog");
        }

        [ConsoleOption(5, "Clear all")]
        public static void ClearAll()
        {
            foreach (var log in EventLog.GetEventLogs())
            {
                Console.WriteLine($"Clearing logs for : {log.Source}, Group: {log.Log}, LogDisplay: {log.LogDisplayName}");
                log.Clear();
            }
        }
    }
}