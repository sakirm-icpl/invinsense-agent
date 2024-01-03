using System;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Common.ConfigProvider;
using ToolManager.Models;
using Serilog;
using Common.Net;
using Microsoft.Win32;
using System.Globalization;
using System.Runtime.InteropServices;
using Common.FileHelpers;

namespace ToolManager
{
    public abstract class ProductManager
    {
        protected readonly ILogger _logger;
        protected readonly ToolDetail _toolDetail;

        public ProductManager(ToolDetail toolDetail, ILogger logger)
        {
            _logger = logger;
            _toolDetail = toolDetail;
        }

        public InstallStatus GetInstallStatus()
        {
            var success = GetInstalledVersion(out InstallStatusWithDetail detail);

            if (!success)
            {
                _logger.Error("Error in detecting version.");
                return InstallStatus.Error;
            }

            var versionDetectionInstruction = _toolDetail.VersionDetectionInstruction;

            var requiredVersion = new Version(versionDetectionInstruction.Version);
            var minVersion = new Version(versionDetectionInstruction.MinVersion);
            var maxVersion = new Version(versionDetectionInstruction.MaxVersion);

            _logger.Information($"version: {detail}, Required: {requiredVersion}, Min: {minVersion}, Max: {maxVersion}");

            if (detail.Version < minVersion)
            {
                _logger.Information("version is less than minimum version.");
                return InstallStatus.Outdated;
            }

            if (detail.Version > maxVersion)
            {
                _logger.Information("version is greater than maximum version.");
                return InstallStatus.UnSupported;
            }

            if (detail.Version == requiredVersion)
            {
                _logger.Information("version is equal to required version.");
                return InstallStatus.Installed;
            }

            return InstallStatus.Installed;
        }

        public int Preinstall()
        {
            var status = GetInstallStatus();

            if (status == InstallStatus.Error || status == InstallStatus.Unknown)
            {
                return -1;
            }

            if (status == InstallStatus.UnSupported)
            {
                return 1;
            }

            if (status == InstallStatus.Installed)
            {
                _logger.Information("Tool is already installed.");
                return 1;
            }

            var downloader = new FragmentedFileDownloader();

            // Download required files from server.
            var downloadUrl = _toolDetail.DownloadUrl;
            _logger.Information($"Downloading {downloadUrl}");

            var destinationFile = Path.Combine(CommonUtils.ArtifactsFolder, $"{_toolDetail.Name}.zip");
            var destinationFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);

            downloader.DownloadFileAsync(downloadUrl, destinationFile).Wait();

            //Extract zip file
            ZipArchiveHelper.ExtractZipFileWithOverwrite(destinationFile, destinationFolder);

            //Remove zip file
            File.Delete(destinationFile);

            _logger.Information($"File downloaded and extracted.");
            return 0;
        }

        public abstract void PostInstall();

        public bool GetInstalledVersion(out InstallStatusWithDetail detail)
        {
            var versionDetectionInstruction = _toolDetail.VersionDetectionInstruction;

            if(versionDetectionInstruction.Type == VersionDetectionType.Unknown)
            {
                detail = new InstallStatusWithDetail
                {
                    InstallStatus = InstallStatus.Unknown,
                };
                return false;
            }

            if (versionDetectionInstruction.Type == VersionDetectionType.ProgramRegistry)
                return GetProductInfoReg(versionDetectionInstruction.Key, out detail);

            if (versionDetectionInstruction.Type == VersionDetectionType.ServiceRegistry)
                return GetServiceInfo(versionDetectionInstruction.Key, out detail);

            detail = new InstallStatusWithDetail
            {
                InstallStatus = InstallStatus.Unknown
            };
            return false;
        }

        public int InstallProduct()
        {
            if (!_toolDetail.IsActive)
            {
                _logger.Information("Installation is not required as tool is not active");
                return 0;
            }

            var toolName = _toolDetail.Name;
            var instruction = _toolDetail.InstallInstructions;

            try
            {
                _logger.Information($"Preparing installation for {toolName}");

                var isInstalledVersionFetched = GetInstalledVersion(out var detail);
                Log.Logger.Information($"Installed version: {detail}");

                var installerFile = instruction.InstallerFile;

                var isNewVersionFetched = MsiPackageWrapper.GetMsiVersion(installerFile, out var newVersion);

                if (!isInstalledVersionFetched || !isNewVersionFetched)
                {
                    _logger.Error("Error fetching file version.");
                    return -1;
                }

                var msiPath = Path.Combine(CommonUtils.ArtifactsFolder, toolName, installerFile);

                _logger.Information($"MSI Path: {msiPath}, version: {newVersion}");

                if (detail.Version != null && newVersion <= detail.Version)
                {
                    _logger.Error($"{toolName} is already installed.");
                    return 0;
                }

                if (!IsMsiInstallerBusy()) return 1;

                _logger.Information("MSI installer is ready");

                var logPath = Path.Combine(CommonUtils.LogsFolder, $"{toolName}-install.log");

                _logger.Information($"{toolName} msiPath {msiPath}");
                _logger.Information($"{toolName} logPath {logPath}");

                var isSuccess = MsiPackageWrapper.Install(msiPath, logPath, instruction.InstallArgs.ToArray());

                if (isSuccess)
                {
                    _logger.Information($"{toolName} installation completed");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }

            return 1;
        }

        protected int UninstallProduct()
        {
            var toolName = _toolDetail.Name;
            var instruction = _toolDetail.InstallInstructions;
            var productName = _toolDetail.VersionDetectionInstruction.Key;

            try
            {
                _logger.Information($"Preparing un-installation for {toolName}");

                var status = false;

                if (instruction.InstallType == InstallType.Installer)
                {
                    if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                    {
                        _logger.Error("MSI installer is not ready.");
                        return 1618;
                    }

                    _logger.Information("MSI installer is ready");

                    _logger.Information($"{productName} uninstall started...");
                    status = MsiPackageWrapper.Uninstall(productName);
                }
                else if (instruction.InstallType == InstallType.Executable)
                {
                    _logger.Information($"{productName} uninstall started...");

                    ExePackageWrapper.Uninstall(instruction.InstallerFile, instruction.UninstallArgs.ToString());
                }
                else
                {
                    _logger.Error($"Install Type {instruction.InstallType} not supported");
                }

                return status ? 0 : 1;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }

        public static bool GetProductInfoReg(string productName, out InstallStatusWithDetail productInfo)
        {
            if (GetProductInfoRegByKey(productName, Architecture.X64, out productInfo)) return true;
            if (GetProductInfoRegByKey(productName, Architecture.X86, out productInfo)) return true;
            return false;
        }

        private static bool GetProductInfoRegByKey(string productName, Architecture architecture, out InstallStatusWithDetail productInfo)
        {
            productInfo = new InstallStatusWithDetail();

            var found = false;

            var regViewType = architecture == Architecture.X64 ? RegistryView.Registry64 : RegistryView.Registry32;

            using (var rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regViewType))
            {
                var uninstallKey = rk.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

                foreach (var skName in uninstallKey.GetSubKeyNames())
                    using (var subkey = uninstallKey.OpenSubKey(skName))
                    {
                        try
                        {
                            var displayName = (string)subkey.GetValue("DisplayName");

                            if (displayName == null || !displayName.Contains(productName)) continue;

                            found = true;
                            productInfo.Name = displayName;
                            productInfo.Version = new Version(subkey.GetValue("DisplayVersion").ToString());
                            productInfo.InstalledDate = ConvertToDateTime((string)subkey.GetValue("InstallDate"));
                            productInfo.InstallPath = (string)subkey.GetValue("InstallLocation");
                            productInfo.FileDate = CommonFileHelpers.GetFileDate(productInfo.InstallPath);
                            productInfo.Architecture = architecture;
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

        public static bool GetServiceInfo(string productName, out InstallStatusWithDetail productInfo)
        {
            productInfo = new InstallStatusWithDetail();

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
        

        // The install date string format is YYYYMMDD
        private static DateTime? ConvertToDateTime(string installDateStr)
        {
            if (DateTime.TryParseExact(installDateStr, "yyyyMMdd", null, DateTimeStyles.None, out var date)) return date;
            return null;
        }

        protected bool IsServiceInstalled(string serviceName)
        {
            var services = ServiceController.GetServices();
            foreach (var service in services)
                if (service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private bool IsMsiInstallerBusy()
        {
            if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
            {
                _logger.Error("MSI installer is not ready. 1618");
                return false;
            }

            return true;
        }
    }
}