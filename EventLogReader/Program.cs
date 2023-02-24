using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Management;

namespace eventsLogReader
{
    class Program
    {
        public static bool IsWindowsDefenderEnabled()
        {
            /*ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM AntivirusProduct");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                string productName = queryObj["displayName"].ToString();
                if (productName == "Windows Defender")
                {
                    string productState = queryObj["productState"].ToString();
                    return (productState == "393472" || productState == "397584");
                }
            }
            return false;*/
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
        static void Main(string[] args) 
        {
            try
            {
                if(IsWindowsDefenderEnabled()==false)
                {
                    Console.WriteLine("Windows Defender is disabled");
                }
                else 
                {
                    Console.WriteLine("Windows Defender is enabled");
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}


