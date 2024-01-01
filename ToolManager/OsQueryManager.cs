using System;
using System.Collections.Generic;
using System.IO;
using Common.ConfigProvider;
using Common.Helpers;
using Common.Net;
using Common.Persistence;
using Serilog;
using ToolManager.Models;

namespace ToolManager
{
    public sealed class OsQueryManager : ProductManager
    {
        public OsQueryManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(OsQueryManager)))
        {
            _logger.Information($"Initializing {nameof(OsQueryManager)} Manager");
        }

        /// <summary>
        /// Download required files
        /// </summary>
        /// <returns></returns>
        public override int Preinstall()
        {
            var success = GetInstalledVersion(out Version version);

            if (!success)
            {
                Console.WriteLine("OsQuery Error in detecting version.");
                return 1;
            }

            Console.WriteLine($"OsQuery version: {version}");

            var osQrv = new Version(_toolDetail.Version);
            var osQmv = new Version(_toolDetail.MinVersion);
            var osQxv = new Version(_toolDetail.MaxVersion);

            var downloader = new FragmentedFileDownloader();

            if (version == null || version < osQmv)
            {
                // Download required files from server.
                Console.WriteLine("OsQuery version is less than minimum version.");

                var downloadUrl = _toolDetail.DownloadUrl;

                Console.WriteLine($"Downloading {downloadUrl}");

                var destinationFile = Path.Combine(CommonUtils.ArtifactsFolder, $"{ToolName.OsQuery}.zip");
                var destinationFolder = Path.Combine(CommonUtils.ArtifactsFolder, ToolName.OsQuery);

                downloader.DownloadFileAsync(downloadUrl, destinationFile).Wait();

                //Extract zip file
                ZipArchiveHelper.ExtractZipFileWithOverwrite(destinationFile, destinationFolder);

                //Remove zip file
                File.Delete(destinationFile);
            }
            else if (version > osQxv)
            {
                Console.WriteLine("OsQuery version is greater than maximum version.");
            }

            if (version == osQrv)
            {
                Console.WriteLine("OsQuery version is equal to required version.");
            }
            return 0;
        }

        public override VersionDetectionInstruction GetVersionDetectionInstruction()
        {
            return new VersionDetectionInstruction
            {
                Type = VersionDetectionType.Registry,
                Path = ToolName.OsQuery,
                Pattern = "version"
            };
        }

        public override InstallInstruction GetInstallInstruction()
        {
            return new InstallInstruction
            {
                Name = _toolDetail.Name,
                WorkingDirectory = CommonUtils.ArtifactsFolder,
                InstallType = InstallType.Installer,
                RequiredVersion = _toolDetail.Version,
                MinimumVersion = _toolDetail.MinVersion,
                MaximumVersion = _toolDetail.MaxVersion,
                InstallArgs = new List<string>
                {
                    "ALLUSERS=1", 
                    "ACCEPTEULA=1"
                },
                UninstallArgs = new List<string>
                {
                }
            };
        }

        public override int Install()
        {
            if (_toolDetail.IsActive)
            {
                _logger.Information("Installation is not required as tool is not active");
                return 0;
            }

            try
            {
                var exitCode = InstallMsi();
                if (exitCode != 0) return exitCode;
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }

        public override void PostInstall()
        {
            var sourcePath = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);

            //Get OSQuery installation path Program Files\osquery
            var installationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), _toolDetail.Name);

            //Copy osquery.conf to installation path
            var configSource = Path.Combine(sourcePath, $"{ToolName.OsQuery}.conf");
            var configDestination = Path.Combine(installationPath, $"{ToolName.OsQuery}.conf");
            EnsureSourceToDestination(configSource, configDestination);

            //Copy packs to installation path
            var packsSourcePath = Path.Combine(sourcePath, $"{ToolName.OsQuery}-packs.zip");
            var packsDestinationPath = Path.Combine(installationPath, "packs");
            ExtractSourceToDestination(packsSourcePath, packsDestinationPath);
        }

        public override int Remove()
        {
            return UninstallMsi();
        }
    }
}