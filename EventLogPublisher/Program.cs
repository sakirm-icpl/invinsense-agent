using System;
using Common;
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

            string str;
            while ((str = Console.ReadLine()) != "q")
            {
                if (str.StartsWith("create"))
                {
                    // Create the source, if it does not already exist.
                    if (EventLog.SourceExists(Constants.IvsAgentName))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(Constants.IvsAgentName, ".")}");

                        EventLog.WriteEntry(Constants.IvsAgentName, "Test message", EventLogEntryType.Warning);

                        EventLog log = new EventLog
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
                    EventLog.CreateEventSource(Constants.IvsAgentName, Constants.LogGroupName);
                    Console.WriteLine("CreatedEventSource");
                    Console.WriteLine("Exiting, execute the application a second time to use the source.");
                    return;
                }

                if (str.StartsWith("delete"))
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

                    if (EventLog.Exists(Constants.LogGroupName))
                    {
                        Console.WriteLine($"Log exists {Constants.LogGroupName}");
                        EventLog.Delete(Constants.LogGroupName);
                    }
                    else
                    {
                        Console.WriteLine("Log not exists");
                    }

                    continue;
                }

                if (str.StartsWith("e3"))
                {
                    // Create the source, if it does not already exist.
                    if (!EventLog.SourceExists("MySource"))
                    {
                        //An event log source should not be created and immediately used.
                        //There is a latency time to enable the source, it should be created
                        //prior to executing the application that uses the source.
                        //Execute this sample a second time to use the new source.
                        EventLog.CreateEventSource("MySource", "MyNewLog");
                        Console.WriteLine("CreatedEventSource");
                        Console.WriteLine("Exiting, execute the application a second time to use the source.");
                        return;
                    }

                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "MySource"
                    };

                    // Write an informational entry to the event log.
                    myLog.WriteEntry("Writing to event log.");
                }

                if (str.StartsWith("e4"))
                {
                    if (EventLog.SourceExists("MySource"))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName("MySource", ".")}");
                        EventLog.DeleteEventSource("MySource");
                    }
                    else
                    {
                        Console.WriteLine("Source not exists");
                    }

                    if (EventLog.Exists("MyNewLog"))
                    {
                        Console.WriteLine($"Log exists MyNewLog");
                        EventLog.Delete("MyNewLog");
                    }
                    else
                    {
                        Console.WriteLine("Log not exists");
                    }
                }

                if (str.StartsWith("clear all"))
                {
                    foreach (var log in EventLog.GetEventLogs())
                    {
                        log.Clear();
                    }
                }
            }
        }
    }
}
