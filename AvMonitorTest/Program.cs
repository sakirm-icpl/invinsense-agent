using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace AvMonitorTest
{
    internal class Program
    {
        static void Main()
        {
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
                if (value != null && value is int && (int)value != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
