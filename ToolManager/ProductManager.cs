using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Common.ConfigProvider;
using Common.Helpers;
using Common.Persistence;
using Common.RegistryHelpers;
using MsiWrapper;
using Serilog;

namespace ToolManager
{
    public abstract class ProductManager
    {
        protected readonly ILogger _logger;

        public ProductManager(ILogger logger)
        {
            _logger = logger;
        }

        protected bool GetInstalledVersion(VersionDetectionInstruction model, out Version version)
        {
            if (model.Type == VersionDetectionType.FilePath)
                return GetVersionFromName(model.Path, out version);

            if (model.Type == VersionDetectionType.FileInfo)
                return GetFileVersion(model.Path, out version);

            if (model.Type == VersionDetectionType.FileContent)
                return GetFileContentVersion(model.Path, model.Pattern, out version);

            if (model.Type == VersionDetectionType.Registry)
                return GetRegistryVersion(model.Path, model.Pattern, out version);

            version = null;
            return false;
        }

        protected int InstallMsi(InstallInstruction instruction, VersionDetectionInstruction vdInstruction)
        {
            try
            {
                _logger.Information($"Preparing installation for {instruction.Name}");

                var isInstalledVersionFetched = GetInstalledVersion(vdInstruction, out var installedVersion);
                Log.Logger.Information($"Installed version: {installedVersion}");

                //get latest msi file from artifacts folder
                var msiPath = GetLatestPath(instruction.Name, "msi");
                if (string.IsNullOrEmpty(msiPath))
                {
                    _logger.Error("Exiting process as MSI file not supplied.");
                    return installedVersion == null ? -1 : 0;
                }

                var isNewVersionFetched = MsiPackageWrapper.GetMsiVersion(msiPath, out var newVersion);

                if (!isInstalledVersionFetched || !isNewVersionFetched)
                {
                    _logger.Error("Error fetching file version.");
                    return -1;
                }

                _logger.Information($"MSI Path: {msiPath}, version: {newVersion}");

                if (installedVersion != null && newVersion <= installedVersion)
                {
                    _logger.Error($"{instruction.Name} is already installed.");
                    File.Delete(msiPath);
                    return 0;
                }

                if (!IsMsiInstallerBusy()) return 1;

                _logger.Information("MSI installer is ready");

                var logPath = Path.Combine(CommonUtils.LogsFolder, $"{instruction.Name}-install.log");

                _logger.Information($"{instruction.Name} msiPath {msiPath}");
                _logger.Information($"{instruction.Name} logPath {logPath}");

                var isSuccess = MsiPackageWrapper.Install(msiPath, logPath, instruction.InstallArgs.ToArray());

                if (isSuccess)
                {
                    _logger.Information($"{instruction.Name} installation completed");
                    File.Delete(msiPath);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }

            return 1;
        }

        protected int UninstallMsi(InstallInstruction instruction)
        {
            try
            {
                _logger.Information($"Preparing un-installation for {instruction.Name}");

                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    _logger.Error("MSI installer is not ready.");
                    return 1618;
                }

                var status = true;

                _logger.Information("MSI installer is ready");

                _logger.Information($"{instruction.Name} uninstall started...");
                status = MsiPackageWrapper.Uninstall(instruction.Name);

                return status ? 0 : 1;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }

        /// <summary>
        ///     Get the latest version of executable from the default path
        /// </summary>
        /// <param name="version"></param>
        /// <returns>true: without error, false: with error</returns>
        private bool GetFileVersion(string executablePath, out Version version)
        {
            version = null;

            try
            {
                if (!File.Exists(executablePath))
                {
                    _logger.Information(executablePath + " not found");
                    return true;
                }

                // Get product version from file properties
                var fileInfo = FileVersionInfo.GetVersionInfo(executablePath);
                var versionString = fileInfo.ProductVersion;

                if (!string.IsNullOrWhiteSpace(versionString))
                    // ProductVersion might include additional text, e.g. "5.10.2 (build 5.10.2-1.win64)"
                    version = new Version(versionString.Split(' ')[0]);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }

            return false;
        }

        /// <summary>
        ///     Get version from file content
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pattern"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool GetFileContentVersion(string filePath, string pattern, out Version version)
        {
            version = null;

            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.Information(filePath + " not found");
                    return true;
                }

                var fileContent = File.ReadAllText(filePath);
                var match = Regex.Match(fileContent, pattern);

                if (match.Success)
                {
                    version = new Version(match.Groups[1].Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }

            return false;
        }

        private static bool GetVersionFromName(string filePath, out Version version)
        {
            // Use a regular expression to find version numbers in the file name
            var match = Regex.Match(filePath, @"(\d+\.\d+\.\d+)");
            if (match.Success)
            {
                version = new Version(match.Groups[1].Value);
                //Log.Information("Found MSI version {0} in file name {1}", version, filePath);
                return true;
            }

            version = null;
            return false;
        }

        private bool GetRegistryVersion(string registryPath, string pattern, out Version version)
        {
            version = null;

            try
            {
                var value = ToolRegistry.GetPropertyByName(registryPath, pattern);
                version = new Version(value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }

            return false;
        }

        protected string GetLatestPath(string toolName, string extension)
        {
            var files = Directory.GetFiles(CommonUtils.ArtifactsFolder, $"{toolName}-*.{extension}");

            _logger.Information($"Found files: {string.Join(",", files)}");

            if (files.Length == 0)
            {
                _logger.Error("No msi file found in artifacts folder");
                return string.Empty;
            }

            if (files.Length > 1)
            {
                _logger.Error("More than one msi file found in artifacts folder");

                // Sort the files by version number in descending order
                Array.Sort(files, (a, b) =>
                {
                    // Extract version numbers from file names using a regular expression
                    var regex = new Regex($@"{toolName}-(\d+\.\d+\.\d+)\.{extension}");
                    var matchA = regex.Match(Path.GetFileName(a));
                    var matchB = regex.Match(Path.GetFileName(b));

                    if (matchA.Success && matchB.Success)
                    {
                        var versionA = new Version(matchA.Groups[1].Value);
                        var versionB = new Version(matchB.Groups[1].Value);

                        return versionB.CompareTo(versionA); // Sort in descending order
                    }

                    return 0; // Default to no change in order
                });

                // Keep the latest file and delete the rest
                for (var i = 1; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                    _logger.Information($"Deleted: {files[i]}");
                }

                _logger.Information("Old files deleted.");
            }
            else
            {
                _logger.Information("No old files to delete.");
            }

            _logger.Information($"Using file: {files[0]}");

            return files[0];
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

        protected void EnsureSourceToDestination(string sourcePath, string destinationPath)
        {
            _logger.Information($"Copying {sourcePath} to {destinationPath}");
            if (!File.Exists(sourcePath))
            {
                _logger.Error($"{sourcePath} not found");
                return;
            }

            File.Copy(sourcePath, destinationPath, true);

            _logger.Information($"Removing {sourcePath}");
            File.Delete(sourcePath);
        }

        protected void ExtractSourceToDestination(string sourcePath, string destinationPath)
        {
            _logger.Information($"Extracting {sourcePath} to {destinationPath}");
            if (!File.Exists(sourcePath))
            {
                _logger.Error($"{sourcePath} not found");
                return;
            }

            ZipArchiveHelper.ExtractZipFileWithOverwrite(sourcePath, destinationPath);

            _logger.Information($"Removing {sourcePath}");
            File.Delete(sourcePath);
        }
    }
}