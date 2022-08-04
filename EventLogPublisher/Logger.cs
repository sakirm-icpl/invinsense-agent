﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EventLogPublisher
{
    /// <summary>
    /// Logging operations
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Note: The actual limit is higher than this, but different Microsoft operating systems actually have different limits. 
        ///       So just use 30,000 to be safe.
        /// </summary>
        private const int MaxEventLogEntryLength = 30000;

        /// <summary>
        /// Constant event log name
        /// </summary>
        private const string EventLogName = "Invinsense";

        /// <summary>
        /// Gets or sets the source/caller. When logging, this logger class will attempt to get the
        /// name of the executing/entry assembly and use that as the source when writing to a log.
        /// In some cases, this class can't get the name of the executing assembly. This only seems
        /// to happen though when the caller is in a separate domain created by its caller. So,
        /// unless you're in that situation, there is no reason to set this. However, if there is
        /// any reason that the source isn't being correctly logged, just set it here when your
        /// process starts.
        /// </summary>
        public static string Source { get; set; }

        /// <summary>
        /// Logs the message, but only if debug logging is true.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="debugLoggingEnabled">if set to <c>true</c> [debug logging enabled].</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogDebug(string message, bool debugLoggingEnabled, string source = "")
        {
            if (!debugLoggingEnabled) 
            { 
                return; 
            }

            Log(message, EventLogEntryType.Information, source);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogInformation(string message, string source = "")
        {
            Log(message, EventLogEntryType.Information, source);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogWarning(string message, string source = "")
        {
            Log(message, EventLogEntryType.Warning, source);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogException(Exception ex, string source = "")
        {
            if (ex == null) { throw new ArgumentNullException("ex"); }

            if (Environment.UserInteractive)
            {
                Console.WriteLine(ex.ToString());
            }

            Log(ex.ToString(), EventLogEntryType.Error, source);
        }

        private static void Log(string message, EventLogEntryType entryType, string source)
        {
            // Note: I got an error that the security log was inaccessible.
            // To get around it, I ran the app as administrator just once, then I could run it from within VS.

            if (string.IsNullOrWhiteSpace(source))
            {
                source = GetSource();
            }

            message = EnsureLogMessageLimit(message);

            var log = new EventLog(EventLogName)
            {
                Source = source
            };

            log.WriteEntry(message, entryType);

            // If we're running a console app, also write the message to the console window.
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Trace:" + message);
            }
        }

        public static void EnsureEventSource(string source = null)
        {
            try
            {
                if(string.IsNullOrEmpty(source))
                {
                    source = GetSource();
                }

                if (!EventLog.GetEventLogs().Any(x => x.LogDisplayName == EventLogName) || !EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, EventLogName);
                }
            }
            catch (Exception ex)
            {
                //No need to handle
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        public static void DeleteEventSource(string sourceName)
        {
            EventLog.DeleteEventSource(sourceName);
        }

        public static void Delete()
        {
            EventLog.Delete(EventLogName);
        }

        private static string GetSource()
        {
            // If the caller has explicitly set a source value, just use it.
            if (!string.IsNullOrWhiteSpace(Source)) 
            { 
                return Source; 
            }

            try
            {
                var assembly = Assembly.GetEntryAssembly();

                // GetEntryAssembly() can return null when called in the context of a unit test project.
                // That can also happen when called from an app hosted in IIS, or even a windows service.

                if (assembly == null)
                {
                    assembly = Assembly.GetExecutingAssembly();
                }


                if (assembly == null)
                {
                    // From http://stackoverflow.com/a/14165787/279516:
                    assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                }

                if (assembly == null) { return "Unknown"; }

                var source = assembly.GetName().Name;

                return source;
            }
            catch
            {
                return "Unknown";
            }
        }

        // Ensures that the log message entry text length does not exceed the event log viewer maximum length of 32766 characters.
        private static string EnsureLogMessageLimit(string logMessage)
        {
            if (logMessage.Length > MaxEventLogEntryLength)
            {
                string truncateWarningText = string.Format(CultureInfo.CurrentCulture, "... | Log Message Truncated [ Limit: {0} ]", MaxEventLogEntryLength);

                // Set the message to the max minus enough room to add the truncate warning.
                logMessage = logMessage.Substring(0, MaxEventLogEntryLength - truncateWarningText.Length);

                logMessage = string.Format(CultureInfo.CurrentCulture, "{0}{1}", logMessage, truncateWarningText);
            }

            return logMessage;
        }
    }
}
