using Serilog;
using System;
using System.Diagnostics;
using System.Linq;

namespace Common
{
    /// <summary>
    /// Logging operations
    /// </summary>
    public class EventLogHelper
    {
        private static readonly ILogger _logger = Log.ForContext<EventLogHelper>();

        protected EventLogHelper()
        {

        }

        public static void AddEvent(EventLogEntryType entryType, string source, EventId eventId)
        {
            try
            {
                var log = new EventLog(Constants.LogGroupName)
                {
                    Source = source
                };

                var eventDetail = TrackingEventProvider.Instance.GetEventDetail(eventId);

                log.WriteEntry(eventDetail.Message, entryType, eventDetail.Id);


                // If we're running a console app, also write the message to the console window.
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Trace:" + eventDetail.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }
        }

        public static void EnsureEventSource(string source)
        {
            try
            {
                if(!EventLog.Exists(Constants.LogGroupName))
                {
                    Console.WriteLine(Constants.LogGroupName + " does not exists");
                }

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, Constants.LogGroupName);
                }
            }
            catch (Exception ex)
            {
                //No need to handle
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(ex.StackTrace);
                    _logger.Error(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        public static void DeleteEventSource(string sourceName)
        {
            try
            {
                if(EventLog.SourceExists(sourceName))
                {
                    // Find the log associated with this source.
                    var logName = EventLog.LogNameFromSourceName(sourceName, ".");

                    // Make sure the source is in the log we believe it to be in.
                    if (logName != Constants.LogGroupName)
                    {
                        Console.WriteLine($"{sourceName} does not belongs to {Constants.LogGroupName}");
                        return;
                    }
                        
                    EventLog.DeleteEventSource(sourceName);
                }
            }
            catch (Exception ex)
            {
                //No need to handle
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(ex.StackTrace);
                    _logger.Error(ex.StackTrace);
                }
            }
        }

        public static void DeleteEventLog()
        {
            try
            {
                EventLog.Delete(Constants.LogGroupName);
            }
            catch (Exception ex)
            {
                //No need to handle
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(ex.StackTrace);
                    _logger.Error(ex.StackTrace);
                }
            }
        }
    }
}
