using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace AvMonitorTest
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("From WMI");

            var objects = WindowsSecurityProtectioin();

            foreach (var obj in objects)
            {
                Console.WriteLine(obj.Key + " " + obj.Value);
            }

            Console.WriteLine($"From windows registry: {IsWindowsDefenderEnabled()}");
        }

        public static Dictionary<string, string> WindowsSecurityProtectioin()
        {
            var values = new Dictionary<string, string>();

            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject virusChecker in data.Cast<ManagementObject>())
            {
                values.Add(virusChecker["displayName"].ToString(), virusChecker["displayVersion"].ToString());
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
