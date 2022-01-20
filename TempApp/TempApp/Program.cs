using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TempApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("List of agents");

            getAntivirusName();


            ////Seqrite Endpoint Security

            //string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            //using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            //{
            //    foreach (string subkey_name in key.GetSubKeyNames())
            //    {
            //        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
            //        {
            //            Console.WriteLine(subkey.Name);
            //            Console.WriteLine(subkey.GetValue("DisplayName"));
            //        }
            //    }
            //}

            Console.WriteLine("");

            Console.WriteLine("Check for directory");
            CheckDirectory();

            Console.WriteLine("");

            Console.WriteLine("Check for registry");
            CheckRegistry();

            Console.WriteLine("----- END -----");

            //try
            //{
            //    ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            //    foreach (ManagementObject mo in mos.Get())
            //    {
            //        Console.WriteLine(mo["Name"]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            



            Console.ReadLine();
        }

        public static void getAntivirusName()
        {
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject virusChecker in data)
            {
                Console.WriteLine(virusChecker["displayName"]);
                Console.WriteLine(virusChecker["instanceGuid"]);
                Console.WriteLine(virusChecker["pathToSignedProductExe"]);
                Console.WriteLine(virusChecker["productState"]);
                Console.WriteLine(virusChecker["timestamp"]);
            }
        }

        public static void CheckDirectory()
        {
            try
            {
                if (Directory.Exists(@"C:\Program Files (x86)\ossec-agent"))
                    Console.WriteLine("wazuh-agent directory exists");
                else
                    Console.WriteLine("wazuh-agent directory not exists");
            }
            catch (Exception) { }
        }

        public static void CheckRegistry()
        {
            try
            {
                RegistryKey winLogonKey = Registry.CurrentUser.OpenSubKey(@"Software\Wazuh, Inc.\Wazuh Agent");
                if (winLogonKey != null)
                    Console.WriteLine("wazuh-agent registry key exists");
                else
                    Console.WriteLine("wazuh-agent registry keynot  exists");
            }
            catch (Exception) { }
        }
    }
}
