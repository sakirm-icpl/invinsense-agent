using System.Management;
using System;
using System.Linq;
using System.Collections.Generic;

namespace IvsAgent.AvHelper
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

    public class AvStatus
    {
        public AvStatus(ManagementObject mo)
        {
            //We can have separate AV Object for better reporting
            AvName = mo["displayName"].ToString();
            InstanceGuid = mo["instanceGuid"].ToString();
            PathToSignedProductExe = mo["pathToSignedProductExe"].ToString();
            ProviderStatus = ConvertToProviderStatus((uint)mo.Properties["ProductState"].Value);
            TimeStamp = mo["timestamp"].ToString();
        }

        public string AvName { get; set; }

        public string InstanceGuid { get; set; }

        public string PathToSignedProductExe { get; set; }

        public ProviderStatus ProviderStatus { get; private set; }

        public bool IsAvEnabled
        {
            get
            {
                return ProviderStatus.AVStatus.HasFlag(AVStatusFlags.Enabled);
            }
        }

        public bool IsAvUptoDate
        {
            get
            {
                return ProviderStatus.SignatureStatus.HasFlag(SignatureStatusFlags.UpToDate);
            }
        }

        public string TimeStamp { get; set; }

        public static unsafe ProviderStatus ConvertToProviderStatus(uint val) => *(ProviderStatus*)&val;

        public override string ToString()
        {
            return $"{nameof(AvName)}: {AvName} {Environment.NewLine}" +
                $"{nameof(InstanceGuid)}: {InstanceGuid} {Environment.NewLine}" +
                $"{nameof(PathToSignedProductExe)}: {PathToSignedProductExe} {Environment.NewLine}" +
                $"{ProviderStatus}: {Environment.NewLine}" +
                $"\t {nameof(ProviderStatus.SignatureStatus)}: {(ProviderStatus.SignatureStatus.HasFlag(SignatureStatusFlags.UpToDate) ? "up to date" : "out of date")}{Environment.NewLine}" +
                $"\t {nameof(ProviderStatus.AVStatus)}: {(ProviderStatus.AVStatus.HasFlag(AVStatusFlags.Enabled) ? "Enabled" : "Disabled")}{Environment.NewLine}" +
                $"\t {nameof(ProviderStatus.SecurityProvider)}: {ProviderStatus.SecurityProvider}{Environment.NewLine}" +
                $"\t {nameof(ProviderStatus.unused)}: {ProviderStatus.unused}{Environment.NewLine}" +
                $"{nameof(TimeStamp)}: {TimeStamp} {Environment.NewLine}";
        }
    }
}
