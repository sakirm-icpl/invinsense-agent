using System.Management;
using System.Linq;
using System.Collections.Generic;

namespace Common.AvHelper
{
    public static class AvMonitor
    {
        public static IEnumerable<AvStatus> ListAvStatuses()
        {
            //Windows Defender
            //393472 (060100) = disabled and up to date
            //397584 (061110) = enabled and out of date
            //397568 (061100) = enabled and up to date

            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            List<AvStatus> avStatuses = new List<AvStatus>();

            foreach (ManagementObject mo in data.OfType<ManagementObject>())
            {
                avStatuses.Add(new AvStatus(mo));
            }

            return avStatuses;
        }
    }
}
