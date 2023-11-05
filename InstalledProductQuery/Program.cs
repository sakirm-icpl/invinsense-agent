using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace InstalledProductQuery
{
    public struct ServiceProductInfo
    {
        public string Name;
        public string Version;
        public string InstallPath;
        public DateTime? FileDate;
        public DateTime? InstalledDate;

        public override string ToString()
        {
            return $"Name: {Name}\nVersion: {Version}\nInstall Path: {InstallPath}\nFileDate Date: {FileDate}\nInstalledDate: {InstalledDate}";
        }
    }

    internal class Program
    {
        static void Main()
        {
            if (GetServiceInfo("Sysmon64", out ServiceProductInfo sysmonInfo))
            {
                Console.WriteLine(sysmonInfo);
            }

            if (GetProductInfoReg("7-Zip", out ServiceProductInfo _7zip))
            {
                Console.WriteLine(_7zip);
            }

            if (GetProductInfoReg("Git", out ServiceProductInfo git))
            {
                Console.WriteLine(git);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static bool GetProductInfoReg(string productName, out ServiceProductInfo productInfo)
        {
            var found = false;

            // You need to check both the 64-bit registry and the 32-bit registry.
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                if (GetProductInfoRegByKey(productName, rk.OpenSubKey(uninstallKey), out productInfo))
                {
                    return true;
                }
            }

            using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                if (GetProductInfoRegByKey(productName, rk.OpenSubKey(uninstallKey), out productInfo))
                {
                    return true;
                }
            }

            return found;
        }

        private static bool GetProductInfoRegByKey(string productName, RegistryKey uninstallKey, out ServiceProductInfo productInfo)
        {
            productInfo = new ServiceProductInfo();
            var found = false;

            foreach (string skName in uninstallKey.GetSubKeyNames())
            {
                using (RegistryKey subkey = uninstallKey.OpenSubKey(skName))
                {
                    try
                    {
                        string displayName = (string)subkey.GetValue("DisplayName");

                        if (displayName == null || !displayName.Contains(productName))
                        {
                            continue;
                        }

                        found = true;
                        productInfo.Name = displayName;
                        productInfo.Version = (string)subkey.GetValue("DisplayVersion");
                        productInfo.InstalledDate = ConvertToDateTime((string)subkey.GetValue("InstallDate"));
                        productInfo.InstallPath = (string)subkey.GetValue("InstallLocation");
                        break;
                    }
                    catch (Exception ex)
                    {
                        // If there is an error, continue with the next subkey
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return found;
        }

        private static bool GetServiceInfo(string productName, out ServiceProductInfo productInfo)
        {
            productInfo = new ServiceProductInfo();

            string registryKeyPath = $@"SYSTEM\CurrentControlSet\Services\{productName}";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
            {
                if (key == null)
                {
                    Console.WriteLine($"Product {productName} not found.");
                    return false;
                }

                productInfo.Name = key.GetValue("DisplayName") as string;
                string imagePath = key.GetValue("ImagePath") as string;
                productInfo.InstallPath = ExtractExecutableFilePath(imagePath);
                productInfo.Version = GetFileVersion(productInfo.InstallPath);
                productInfo.FileDate = GetFileDate(productInfo.InstallPath);
            }

            return true;
        }

        private static string ExtractExecutableFilePath(string path)
        {
            string absoluteImagePath = Regex.Replace(path, "%(.*?)%", m => Environment.GetEnvironmentVariable(m.Groups[1].Value));

            if (absoluteImagePath.Length == 0)
            {
                return "";
            }

            return (absoluteImagePath[0] == '\"') ? absoluteImagePath.Split('\"')[1] : absoluteImagePath.Split(' ')[0];
        }

        private static string GetFileVersion(string path)
        {
            if (!File.Exists(path))
            {
                return "";
            }

            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
            return versionInfo.ProductVersion;
        }

        private static DateTime? GetFileDate(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.GetCreationTime(path);
        }

        private static DateTime? ConvertToDateTime(string installDateStr)
        {
            // The install date string format is YYYYMMDD
            if (DateTime.TryParseExact(installDateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            return null;
        }
    }
}
