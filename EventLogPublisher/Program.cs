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
                if(str.StartsWith("create"))
                {
                    EventLog.CreateEventSource(Constants.SingleAgentLogSourceName, Constants.LogGroupName);
                    continue;
                }

                if(str.StartsWith("delete"))
                {
                    if (EventLog.SourceExists(Constants.SingleAgentLogSourceName))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(Constants.SingleAgentLogSourceName, ".")}");
                        EventLog.DeleteEventSource(Constants.SingleAgentLogSourceName);
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

                if (str.StartsWith("check"))
                {
                    if(EventLog.Exists(Constants.LogGroupName))
                    {
                        Console.WriteLine("Log exists");
                    }
                    else
                    {
                        Console.WriteLine("Log not exists");
                    }

                    if(EventLog.SourceExists(Constants.SingleAgentLogSourceName))
                    {
                        Console.WriteLine($"Source exists from {EventLog.LogNameFromSourceName(Constants.SingleAgentLogSourceName, ".")}");
                    }
                    else
                    {
                        Console.WriteLine("Source not exists");
                    }

                    continue;
                }

                if(str.StartsWith("entry"))
                {
                    EventLog.WriteEntry(Constants.SingleAgentLogSourceName, "Test message");
                }

                if(str.StartsWith("entry2"))
                {
                    var log = new EventLog(Constants.LogGroupName)
                    {
                        Source = Constants.SingleAgentLogSourceName
                    };

                    log.WriteEntry("Test entry", EventLogEntryType.Information);
                }
            }
        }
    }
}
