using System;
using System.IO;
using Common.ConfigProvider;
using Serilog;
using Common.Net;
using Common.FileHelpers;
using Common.RegistryHelpers;
using Common.Models;
using Common.ServiceHelpers;

namespace ToolManager
{
    /// <summary>
    /// TODO: Gather facts about installation as well as dependencies.
    /// </summary>
    public abstract class ProductManager
    {
        protected readonly ILogger _logger;
        protected readonly ToolDetail _toolDetail;

        public ProductManager(ToolDetail toolDetail, ILogger logger)
        {
            _logger = logger;
            _toolDetail = toolDetail;
        }

        public void Ensure()
        {
            var success = GetInstalledVersion(out InstallStatusWithDetail detail);

            if (success && detail.InstallStatus == InstallStatus.Installed)
            {
                var requiredVersion = new Version(_toolDetail.Version);
                var minVersion = new Version(_toolDetail.UpgradeInstruction.MinVersion);
                var maxVersion = new Version(_toolDetail.DowngradeInstruction.MaxVersion);

                _logger.Information($"version: {detail}, Required: {requiredVersion}, Min: {minVersion}, Max: {maxVersion}");

                if (detail.Version >= minVersion && detail.Version <= maxVersion)
                {
                    _logger.Information($"{_toolDetail.Name} is already installed and is in supported version range.");
                    SyncDatabase();
                    return;
                }

                if (detail.Version < minVersion && _toolDetail.UpgradeInstruction.UnInstallBeforeUpgrade)
                {
                    _logger.Information($"Uninstalling {_toolDetail.Name} before upgrade.");
                    BeforeUninstall(detail);
                    UninstallProduct();
                }
                else if (detail.Version > maxVersion && _toolDetail.DowngradeInstruction.UnInstallBeforeDowngrade)
                {
                    _logger.Information($"Uninstalling {_toolDetail.Name} before downgrade.");
                    BeforeUninstall(detail);
                    UninstallProduct();
                }
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

            InstallProduct();

            SyncDatabase();
        }

        private void SyncDatabase()
        {
            var lastUpdate = WinRegistryHelper.GetPropertyByName(Common.Constants.CompanyName, $"{_toolDetail.Name}_last_update");

            var lastUpdateTime = lastUpdate == null ? DateTime.MinValue : DateTime.Parse(lastUpdate);

            _logger.Information($"Last Update Time: {lastUpdateTime}, Database Update Time: {_toolDetail.UpdatedOn}");

            if (_toolDetail.UpdatedOn >= lastUpdateTime)
            {
                _logger.Information($"{_toolDetail.Name} PostInstall required.");
                PostInstall();
                WinRegistryHelper.SetPropertyByName(Common.Constants.CompanyName, $"{_toolDetail.Name}_last_update", DateTime.Now.ToString());
            }
        }

        protected abstract void PostInstall();

        protected abstract void BeforeUninstall(InstallStatusWithDetail detail);

        public bool GetInstalledVersion(out InstallStatusWithDetail detail)
        {
            var versionDetectionInstruction = _toolDetail.InstallCheckInstruction;

            if (versionDetectionInstruction.Type == VersionDetectionType.Unknown)
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

        private int InstallProduct()
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

                _logger.Information($"Installed version: {detail}");

                var installerFile = Path.Combine(CommonUtils.ArtifactsFolder, toolName, instruction.InstallerFile);

                var isNewVersionFetched = false;
                Version newVersion;

                if (instruction.InstallType == InstallType.Executable)
                {
                    isNewVersionFetched = CommonFileHelpers.GetFileVersion(installerFile, out newVersion);
                    _logger.Information($"Executable Path: {installerFile}, version: {newVersion}");
                }
                else if (instruction.InstallType == InstallType.Installer)
                {
                    isNewVersionFetched = MsiPackageWrapper.GetMsiVersion(installerFile, out newVersion);
                    _logger.Information($"Executable Path: {installerFile}, version: {newVersion}");
                }
                else
                {
                    _logger.Error($"Install Type {instruction.InstallType} not supported");
                    return -1;
                }

                if (!isInstalledVersionFetched || !isNewVersionFetched)
                {
                    _logger.Error("Error fetching file version.");
                    return -1;
                }

                _logger.Information($"Installer Path: {installerFile}, version: {newVersion}");

                if (detail.Version != null && newVersion <= detail.Version)
                {
                    _logger.Information($"{toolName} is already installed.");
                    return 0;
                }

                var isSuccess = false;

                var args = VariableHelper.PrepareArgs(instruction.InstallArgs.ToArray());

                if (instruction.InstallType == InstallType.Executable)
                {
                    isSuccess = ExePackageWrapper.Install(installerFile, args);
                }
                else if (instruction.InstallType == InstallType.Installer)
                {
                    var logPath = Path.Combine(CommonUtils.LogsFolder, $"{toolName}-install.log");
                    isSuccess = MsiPackageWrapper.Install(installerFile, logPath, args);
                }
                else
                {
                    _logger.Error($"Install Type {instruction.InstallType} not supported");
                }

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

        private int UninstallProduct()
        {
            var toolName = _toolDetail.Name;
            var instruction = _toolDetail.InstallInstruction;
            var productName = _toolDetail.InstallCheckInstruction.Key;

            try
            {
                _logger.Information($"{toolName} - {productName} uninstall started...");

                var status = false;

                if (instruction.InstallType == InstallType.Installer)
                {
                    status = MsiPackageWrapper.Uninstall(productName);
                }
                else if (instruction.InstallType == InstallType.Executable)
                {
                    var args = VariableHelper.PrepareArgs(instruction.UninstallArgs.ToArray());
                    ExePackageWrapper.Uninstall(instruction.InstallerFile, args);
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