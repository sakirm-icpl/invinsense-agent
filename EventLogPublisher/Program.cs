using Common;
using System;
using System.Diagnostics;

namespace EventLogPublisher
{
    /// <summary>
    ///     PowerShell Commands:
    ///     Remove-EventLog -LogName "Invinsense"
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     default constructor
        /// </summary>
        protected Program()
        {
        }

        private static void Main()
        {
            Console.WriteLine("Press \"q\" to stop");

            string str;
            while ((str = Console.ReadLine()) != "q")
            {
                if (str.StartsWith("create"))
                {
                    // Create the source, if it does not already exist.
                    if (EventLog.SourceExists(Constants.IvsAgentName))
                    {
                        Console.WriteLine(
                            $"Source exists from {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");

                        EventLog.WriteEntry(Constants.IvsAgentName, "Test message", EventLogEntryType.Warning);

                        var log = new EventLog
                        {
                            Source = Constants.IvsAgentName
                        };

                        log.WriteEntry("Test entry", EventLogEntryType.Information);

                        continue;
                    }

                    //An event log source should not be created and immediately used.
                    //There is a latency time to enable the source, it should be created
                    //prior to executing the application that uses the source.
                    //Execute this sample a second time to use the new source.
                    EventLog.CreateEventSource(Constants.IvsAgentName, Constants.CompanyName);
                    Console.WriteLine("CreatedEventSource");
                    Console.WriteLine("Exiting, execute the application a second time to use the source.");
                    return;
                }

                if (str.StartsWith("delete"))
                {
                    if (EventLog.SourceExists(Constants.IvsAgentName))
                    {
                        Console.WriteLine(
                            $"Source exists from {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");
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

                    continue;
                }

                if (str.StartsWith("test source"))
                {
                    if (EventLog.SourceExists("MySource"))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName("MySource", ".")}");
                        EventLog.DeleteEventSource("MySource");
                        EventLog.Delete("MyNewLog");
                        continue;
                    }

                    Console.WriteLine("Source not exists. creating...");
                    EventLog.CreateEventSource("MySource", "MyNewLog");
                    continue;
                }

                if (str.StartsWith("clear all"))
                    foreach (var log in EventLog.GetEventLogs())
                    {
                        Console.WriteLine($"Clearing logs for : {log.Source}, Grouop: {log.Log}, LogDisplay: {log.LogDisplayName}");
                        log.Clear();
                    }
            }
        }
    }
}