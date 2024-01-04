using System;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using ToolManager.Models;
using Microsoft.Win32;
using Common.FileHelpers;
using Serilog;

namespace ToolManager
{
    public static class ServiceHelper
    {
        private static readonly ILogger logger = Log.ForContext(typeof(ServiceHelper));

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
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var subKey = baseKey.OpenSubKey(registryKeyPath, false)) // False is important!
            {
                if (subKey == null)
                {
                    logger.Information($"Product {productName} not found.");
                    return true;
                }

                productInfo.Name = subKey.GetValue("DisplayName") as string;
                var imagePath = subKey.GetValue("ImagePath") as string;
                productInfo.InstallPath = ExtractExecutableFilePath(imagePath);
                productInfo.FileDate = CommonFileHelpers.GetFileDate(productInfo.InstallPath); 
                var success = CommonFileHelpers.GetFileVersion(productInfo.InstallPath, out var version);
                if (!success)
                {
                    logger.Information($"Product {productName} not found.");
                    return false;
                }

                productInfo.InstallStatus = InstallStatus.Installed;
                productInfo.Version = version;                
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