using System;
using System.IO;
using Common.ConfigProvider;
using Common.FileHelpers;
using ToolManager.Models;
using Serilog;

namespace ToolManager
{
    public sealed class OsQueryManager : ProductManager
    {
        public OsQueryManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(OsQueryManager)))
        {
            _logger.Information($"Initializing {nameof(OsQueryManager)} Manager");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Preinstall()
        {
            return base.Preinstall();
        }

        /// <summary>
        /// 1. Copy osquery.conf to installation path
        /// 2. Copy packs to installation path
        /// </summary>
        /// <returns></returns>
        public override int PostInstall()
        {
            var sourceFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);
            var destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), _toolDetail.Name);

            _logger.Information($"Source: {sourceFolder}, Destination: {destinationFolder}");

            //Copy osquery.conf to installation path
            var configSource = Path.Combine(sourceFolder, $"{ToolName.OsQuery}.conf");
            var configDestination = Path.Combine(destinationFolder, $"{ToolName.OsQuery}.conf");
            CommonFileHelpers.EnsureSourceToDestination(configSource, configDestination);

            //Copy packs to installation path
            var packsSourcePath = Path.Combine(sourceFolder, $"{ToolName.OsQuery}-packs.zip");
            var packsDestinationPath = Path.Combine(destinationFolder, "packs");
            CommonFileHelpers.ExtractSourceToDestination(packsSourcePath, packsDestinationPath);

            return base.PostInstall();
        }
    }
}