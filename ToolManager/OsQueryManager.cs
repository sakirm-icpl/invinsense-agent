using System;
using System.Collections.Generic;
using System.IO;
using Common.ConfigProvider;
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

        public override int Preinstall()
        {
            return 0;
        }

        public override VersionDetectionInstruction GetVersionDetectionInstruction()
        {
            return new VersionDetectionInstruction
            {
                Type = VersionDetectionType.Registry,
                Path = "osquery",
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

            //Copy osqueryd.conf to installation path
            var configSource = Path.Combine(sourcePath, "osquery.conf");
            var configDestination = Path.Combine(installationPath, "osquery.conf");
            EnsureSourceToDestination(configSource, configDestination);

            //Copy packs to installation path
            var packsSourcePath = Path.Combine(sourcePath, "osquery-packs.zip");
            var packsDestinationPath = Path.Combine(installationPath, "packs");
            ExtractSourceToDestination(packsSourcePath, packsDestinationPath);
        }

        public override int Remove()
        {
            return UninstallMsi();
        }
    }
}