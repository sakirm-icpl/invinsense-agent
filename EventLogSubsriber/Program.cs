using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Management;
using System.Text;
using log4net;
using log4net.Config;

namespace MonitorTest
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string log = "Invinsense";
            EventLog demoLog = new EventLog(log);
            EventLogEntryCollection entries = demoLog.Entries;
            foreach (EventLogEntry entry in entries)
            {
                Console.WriteLine("Level: {0}", entry.EntryType);
                Console.WriteLine("Event id: {0}", entry.InstanceId);
                Console.WriteLine("Message: {0}", entry.Message);
                Console.WriteLine("Source: {0}", entry.Source);
                Console.WriteLine("Date: {0}", entry.TimeGenerated);
                Console.WriteLine("--------------------------------");

            }
            Console.ReadLine();
        }
    }
}

