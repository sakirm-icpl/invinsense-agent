using System.Management;
using System;

namespace Common.AvHelper
{
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

        public bool IsAvDisabled
        {
            get
            {
                return ProviderStatus.AVStatus.HasFlag(AVStatusFlags.Disable);
            }
        }
        public bool IsAvUpToDate
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
