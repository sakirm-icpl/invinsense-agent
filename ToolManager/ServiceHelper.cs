using System;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using ToolManager.Models;
using Microsoft.Win32;
using Common.FileHelpers;

namespace ToolManager
{
    public static class ServiceHelper
    {
        public static bool IsServiceInstalled(string serviceName)
        {
            var services = ServiceController.GetServices();
            foreach (var service in services)
                if (service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        public static bool GetServiceInfo(string productName, out InstallStatusWithDetail productInfo)
        {
            productInfo = new InstallStatusWithDetail
            {
                InstallStatus = InstallStatus.NotFound
            };

            var registryKeyPath = $@"SYSTEM\CurrentControlSet\Services\{productName}";
            using (var key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
            {
                if (key == null)
                {
                    Console.WriteLine($"Product {productName} not found.");
                    return false;
                }

                productInfo.Name = key.GetValue("DisplayName") as string;
                var imagePath = key.GetValue("ImagePath") as string;
                productInfo.InstallPath = ExtractExecutableFilePath(imagePath);
                productInfo.Version = CommonFileHelpers.GetFileVersion(productInfo.InstallPath);
                productInfo.FileDate = CommonFileHelpers.GetFileDate(productInfo.InstallPath);
            }

            return true;
        }

        private static string ExtractExecutableFilePath(string path)
        {
            var absoluteImagePath = Regex.Replace(path, "%(.*?)%", m => Environment.GetEnvironmentVariable(m.Groups[1].Value));

            if (absoluteImagePath.Length == 0) return "";

            return absoluteImagePath[0] == '\"' ? absoluteImagePath.Split('\"')[1] : absoluteImagePath.Split(' ')[0];
        }
    }
}