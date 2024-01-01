using System;
using System.Collections.Generic;
using Common.ConfigProvider;
using Common.Persistence;
using Serilog;
using ToolManager.Models;

namespace ToolManager
{
    public sealed class WazuhManager : ProductManager
    {
        public WazuhManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(WazuhManager)))
        {
            _logger.Information($"Initializing {nameof(WazuhManager)} Manager");
        }

        public override VersionDetectionInstruction GetVersionDetectionInstruction()
        {
            return new VersionDetectionInstruction
            {
                Type = VersionDetectionType.Registry,
                Path = ToolName.Wazuh,
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
           
        }

        public override int Remove()
        {
            return UninstallMsi();
        }
    }
}