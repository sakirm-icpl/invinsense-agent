using System;
using System.Management;

namespace SystemManagementTest
{
    internal class MgmtEvtWatcher
    {
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
