using System.Management;
using System;
using System.Linq;
using Common.Persistance;
using System.Runtime.InteropServices;

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

            var av_searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntivirusProduct");
            foreach (ManagementObject info in av_searcher.Get())
            {
                Console.WriteLine(info.Properties["displayName"].Value.ToString());

                var ps = ConvertToProviderStatus((uint)info.Properties["ProductState"].Value);
                Console.WriteLine(ps.SecurityProvider.ToString());
                Console.WriteLine(ps.AVStatus.HasFlag(AVStatusFlags.Enabled) ? "Enabled" : "Disabled");
                Console.Write("Signatures are ");
                Console.WriteLine(ps.SignatureStatus.HasFlag(SignatureStatusFlags.UpToDate) ? "up to date" : "out of date");
                Console.WriteLine();
            }


            return InstallStatus.NotFound;
        }

        public static unsafe ProviderStatus ConvertToProviderStatus(uint val) => *(ProviderStatus*)&val;
    }

    [Flags]
    public enum ProviderFlags : byte
    {
        FIREWALL = 1,
        AUTOUPDATE_SETTINGS = 2,
        ANTIVIRUS = 4,
        ANTISPYWARE = 8,
        INTERNET_SETTINGS = 16,
        USER_ACCOUNT_CONTROL = 32,
        SERVICE = 64,
        NONE = 0,
    }

    [Flags]
    public enum AVStatusFlags : byte
    {
        Unknown = 1,
        Enabled = 16
    }

    [Flags]
    public enum SignatureStatusFlags : byte
    {
        UpToDate = 0,
        OutOfDate = 16
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProviderStatus
    {
        public SignatureStatusFlags SignatureStatus;
        public AVStatusFlags AVStatus;
        public ProviderFlags SecurityProvider;
        public byte unused;
    }
}
