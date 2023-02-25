using System;
using System.Management;
using System.Net.NetworkInformation;

namespace AvMonitorTest
{
    internal class Program
    {
        static void Main()
        {
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject virusChecker in data)
            {
                Console.WriteLine("Antivirus software found: " + virusChecker["displayName"]);
                Console.WriteLine("Antivirus software version: " + virusChecker["displayVersion"]);
            }
        }
    }
}
