using System.Management;
using System;
using System.Linq;
using Common.Persistance;

namespace AvMonitorTest
{
    public static class AvMonitor
    {
        public static InstallStatus AVStatus(string avName)
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
                        return InstallStatus.NotFound;
                    }
                    else if (mo["productState"].ToString() == "397584")
                    {
                        return InstallStatus.Outdated;
                    }
                    else if (mo["productState"].ToString() == "397568")
                    {
                        return InstallStatus.Installed;
                    }
                }

                //We can have separate AV Object for better reporting
                Console.WriteLine(mo["displayName"]);
                Console.WriteLine(mo["instanceGuid"]);
                Console.WriteLine(mo["pathToSignedProductExe"]);
                Console.WriteLine(mo["productState"]);
                Console.WriteLine(mo["timestamp"]);
            }

            return InstallStatus.NotFound;
        }
    }
}
