﻿using Common.ConfigProvider;
using Common.Persistence;
using Serilog;
using System;
using System.Collections.Generic;
using ToolManager.Models;

namespace ToolManager
{
    /// <summary>
    /// https://www.blumira.com/enable-sysmon/
    /// 
    /// </summary>
    public sealed class SysmonManager : ProductManager
    {
        public SysmonManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(SysmonManager)))
        {
            _logger.Information($"Initializing {nameof(SysmonManager)} Manager");
        }

        public override VersionDetectionInstruction GetVersionDetectionInstruction()
        {
            return new VersionDetectionInstruction
            {
                Type = VersionDetectionType.Registry,
                Path = ToolName.Sysmon,
                Pattern = "version"
            };
        }

        public override InstallInstruction GetInstallInstruction()
        {
            return new InstallInstruction
            {
                Name = _toolDetail.Name,
                WorkingDirectory = CommonUtils.ArtifactsFolder,
                InstallType = InstallType.Executable,
                RequiredVersion = _toolDetail.Version,
                MinimumVersion = _toolDetail.MinVersion,
                MaximumVersion = _toolDetail.MaxVersion,
                InstallArgs = new List<string>
                {
                   
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
            return -1;
        }
    }
}