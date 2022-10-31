using System;
using System.IO;
using System.Linq;
using System.Management;

namespace Common.Extensions
{
    public class UserExtensions
    {
        public static string GetCurrentUserName()
        {
            SelectQuery query = new SelectQuery(@"Select * from Win32_Process");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Process in searcher.Get().Cast<ManagementObject>())
                {
                    if (Process["ExecutablePath"] != null && string.Equals(Path.GetFileName(Process["ExecutablePath"].ToString()), "explorer.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] OwnerInfo = new string[2];
                        Process.InvokeMethod("GetOwner", OwnerInfo);

                        return OwnerInfo[0];
                    }
                }
            }
            return "";
        }
    }
}
