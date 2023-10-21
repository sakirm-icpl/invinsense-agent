using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System.Threading;

namespace AvMonitorTest
{
    /// <summary>
    /// https://erikengberg.com/4-ways-to-monitor-windows-registry-using-c/#Method_3_Process_Hooking
    /// https://learn.microsoft.com/en-us/windows/win32/api/_etw/
    /// https://learn.microsoft.com/en-us/windows/win32/api/winreg/nf-winreg-regnotifychangekeyvalue?redirectedfrom=MSDN
    /// </summary>
    internal class Program
    {
        static void Main()
        {
            KernelEventTracer();
            Console.ReadLine();

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
            {
                if (key != null)
                {
                    object value = key.GetValue("TEST");
                    Console.WriteLine($"'TEST' key has been modified. New value: {value}");
                }
            }

            var test = Environment.GetEnvironmentVariable("TEST", EnvironmentVariableTarget.Machine);
            Console.WriteLine($"TEST: {test}");

            WatchEnvironmentVariable();

            Console.WriteLine("From WMI");

            var objects = WindowsSecurityProtection();

            foreach (var obj in objects)
            {
                Console.WriteLine(obj.Key + " " + obj.Value);
            }

            Console.WriteLine($"From windows registry: {IsWindowsDefenderEnabled()}");

            //Invoke windows shell command to send process "msg * /v Test Message!"

            Console.WriteLine("Press any key to send message to all users");
            Console.ReadLine();
            Process.Start("cmd.exe", "/c msg * Test Message!");


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        public static Dictionary<string, string> WindowsSecurityProtection()
        {
            var values = new Dictionary<string, string>();

            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject mo in data.Cast<ManagementObject>())
            {
                var displayName = mo["displayName"].ToString();
                var lastUpdated = mo["timestamp"].ToString();

                values.Add(displayName, lastUpdated);
            }

            return values;
        }

        public static bool IsWindowsDefenderEnabled()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender");
            if (key != null)
            {
                object value = key.GetValue("DisableRealtimeMonitoring");
                if (value != null && value is int v && v != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static void WatchEnvironmentVariable()
        {
            // set up a scope and a query to watch for changes to the registry key
            ManagementScope scope = new ManagementScope("\\\\.\\root\\default");

            // WMI query to watch for changes to the specified registry key
            WqlEventQuery query = new WqlEventQuery(
                "SELECT * FROM RegistryKeyChangeEvent " +
                "WHERE Hive = 'HKEY_LOCAL_MACHINE'" +
                "AND KeyPath = 'SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment'");

            // Initialize watcher and set query
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, query);

            // Event handler to be called when the registry key changes
            watcher.EventArrived += (sender, args) =>
            {
                // Read the updated value of the "TEST" key
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("TEST");
                        Console.WriteLine($"'TEST' key has been modified. New value: {value}");
                    }
                }
            };

            // Start watching
            watcher.Start();

            Console.WriteLine("Watching for changes... Press any key to exit.");
            Console.ReadKey();

            // Stop watching
            watcher.Stop();

        }

        static void KernelEventTracer()
        {
            // Run as Admin
            using (var session = new TraceEventSession("MyKernelSession", KernelTraceEventParser.KernelSessionName))
            {
                // Enable registry events
                session.EnableKernelProvider(KernelTraceEventParser.Keywords.Registry);

                Console.WriteLine("Monitoring registry changes. Press any key to exit...");

                // Subscribe to the event
                session.Source.Kernel.RegistrySetValue += data =>
                {
                    // Check for our specific registry key
                    //if (data.KeyPath.Contains(@"CurrentControlSet\\Control\\Session Manager\\Environment"))
                    {
                        Console.WriteLine($"Registry Key Modified: {data.ActivityID}");
                        Console.WriteLine($"Value Name: {data.ValueName}");
                        Console.WriteLine($"New Value: {BitConverter.ToString(data.EventData())}");
                    }
                };

                // Process the events
                session.Source.Process();

                // Close when a key is pressed
                Console.ReadKey();
            }
        }
    }
}
