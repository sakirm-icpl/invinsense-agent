using System;
using System.IO;
using Common.ConfigProvider;
using Common.FileHelpers;
using Serilog;
using Common.RegistryHelpers;
using Common.Models;

namespace ToolManager
{
    public sealed class OsQueryManager : ProductManager
    {
        public OsQueryManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(OsQueryManager)))
        {
            _logger.Information($"Initializing {nameof(OsQueryManager)} Manager");
        }

        /// <summary>
        /// TODO: Can be used to gather facts about OSQuery installation
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected override int PreInstall(int status)
        {
            return status;
        }

        /// <summary>
        /// 1. Copy osquery.conf to installation path
        /// 2. Copy packs to installation path
        /// </summary>
        /// <returns></returns>
        protected override int PostInstall(int status)
        {
            if (status != 0) return status;

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

            WinRegistryHelper.SetPropertyByName("Infopercept", "osquery_last_update", DateTime.Now.ToString());

            return status;
        }
    }
}