using System;
using System.Runtime.InteropServices;

namespace Common.Models
{
    public struct InstallStatusWithDetail
    {
        public string Name;
        public InstallStatus InstallStatus;
        public Version Version;
        public Architecture Architecture;
        public string InstallPath;
        public DateTime? FileDate;
        public DateTime? InstalledDate;

        public override string ToString()
        {
            return $"Name: {Name} Version: {Version} Install Path: {InstallPath} Architecture: {Architecture} FileDate Date: {FileDate} InstalledDate: {InstalledDate}";
        }
    }
}