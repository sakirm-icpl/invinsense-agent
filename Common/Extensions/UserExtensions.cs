﻿using System;
using System.IO;
using System.Management;

namespace Common.Extensions
{
    public class UserExtensions
    {
        public static string getUserName()
        {
            SelectQuery query = new SelectQuery(@"Select * from Win32_Process");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Process in searcher.Get())
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
