using System;
using System.IO;
using Common.ConfigProvider;
using ToolManager.Models;
using Serilog;
using Common.Net;
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

        public virtual int Preinstall()
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

        public virtual int PostInstall()
        {
            return 0;
        }

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
                return MsiPackageWrapper.GetProductInfoReg(versionDetectionInstruction.Key, out detail);

            if (versionDetectionInstruction.Type == VersionDetectionType.ServiceRegistry)
                return ServiceHelper.GetServiceInfo(versionDetectionInstruction.Key, out detail);

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
            var instruction = _toolDetail.InstallInstruction;

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

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Error("MSI installer is not ready. 1618");
                    return 1618;
                }

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
            var instruction = _toolDetail.InstallInstruction;
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
    }
}