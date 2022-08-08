﻿using System.ServiceProcess;
using System.Management;
using System;
using System.Linq;
using Common;

namespace Invinsense30.Monitor
{
    public static class ServiceHelper
    {
        public static string GetServiceStatus(string servicename)
        {
            ServiceController sc = new ServiceController(servicename);
            return sc.Status.ToString();
        }

        public static EventId AVStatus(string avName)
        {
            //Windows Defender
            //393472 (060100) = disabled and up to date
            //397584 (061110) = enabled and out of date
            //397568 (061100) = enabled and up to date

            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject mo in data.OfType<ManagementObject>())
            {
                if (avName == mo["displayName"].ToString())
                {
                    if (mo["productState"].ToString() == "393472")
                    {
                        return EventId.AvDisabled;
                    }
                    else if (mo["productState"].ToString() == "397584")
                    {
                        return EventId.AvEnabledOutDated;
                    }
                    else if (mo["productState"].ToString() == "397568")
                    {
                        return EventId.AvEnabledUpToDate;
                    }
                }

                //We can have separate AV Object for better reporting
                Console.WriteLine(mo["displayName"]);
                Console.WriteLine(mo["instanceGuid"]);
                Console.WriteLine(mo["pathToSignedProductExe"]);
                Console.WriteLine(mo["productState"]);
                Console.WriteLine(mo["timestamp"]);
            }

            return EventId.AvNotFound;
        }

        public static void RunManagementEventWatcherForWindowsServices()
        {
            EventQuery eventQuery = new EventQuery
            {
                QueryString = "SELECT * FROM __InstanceModificationEvent within 2 WHERE targetinstance isa 'Win32_Service'"
            };

            ManagementEventWatcher demoWatcher = new ManagementEventWatcher(eventQuery);
            demoWatcher.Options.Timeout = new TimeSpan(1, 0, 0);
            Console.WriteLine("Perform the appropriate change in a Windows service according to your query");
            ManagementBaseObject nextEvent = demoWatcher.WaitForNextEvent();
            ManagementBaseObject targetInstance = (ManagementBaseObject)nextEvent["targetinstance"];
            PropertyDataCollection props = targetInstance.Properties;
            foreach (PropertyData prop in props)
            {
                Console.WriteLine("Property name: {0}, property value: {1}", prop.Name, prop.Value);
            }

            demoWatcher.Stop();
        }
    }
}
