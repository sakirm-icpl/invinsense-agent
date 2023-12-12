using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace AvMonitorTest
{
    public static class AvMonitor
    {
        public static IEnumerable<AvStatus> ListAvStatuses()
        {
            //Windows Defender
            //393472 (060100) = disabled and up to date
            //397584 (061110) = enabled and out of date
            //397568 (061100) = enabled and up to date

            var wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            var data = wmiData.Get();

            var avStatuses = new List<AvStatus>();

            foreach (var mo in data.OfType<ManagementObject>()) avStatuses.Add(new AvStatus(mo));

            return avStatuses;
        }
    }

    public struct AvStatus
    {
        public AvStatus(ManagementObject mo)
        {
            //We can have separate AV Object for better reporting
            AvName = mo["displayName"].ToString();
            AvVersion = mo["displayVersion"].ToString();
            InstanceGuid = mo["instanceGuid"].ToString();
            PathToSignedProductExe = mo["pathToSignedProductExe"].ToString();
            ProviderStatus = ConvertToProviderStatus((uint)mo.Properties["ProductState"].Value);
            TimeStamp = mo["timestamp"].ToString();
        }


        public string AvName { get; set; }

        public string AvVersion { get; set; }

        public string InstanceGuid { get; set; }

        public string PathToSignedProductExe { get; set; }

        public ProviderStatus ProviderStatus { get; }

        public string TimeStamp { get; set; }

        public static unsafe ProviderStatus ConvertToProviderStatus(uint val)
        {
            return *(ProviderStatus*)&val;
        }

        public override string ToString()
        {
            return $"{nameof(AvName)}: {AvName} {Environment.NewLine}" +
                   $"{nameof(AvVersion)}: {AvVersion} {Environment.NewLine}" +
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