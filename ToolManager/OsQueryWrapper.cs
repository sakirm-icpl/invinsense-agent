using Common.Utils;
using Serilog;
using System;
using System.IO;
using Common.Persistence;

namespace ToolManager
{
    public sealed class OsQueryWrapper : ProductManager
    {
        public OsQueryWrapper() : base(Log.ForContext(typeof(OsQueryWrapper)), ToolDescriptors.OsQuery)
        {
            _logger.Information($"Initializing {ToolDescriptor.Name} wrapper");
        }

        public int EnsureInstall()
        {
            try
            {
                var exitCode = InstallMsi();
                if (exitCode != 0)
                {
                    return exitCode;
                }

                if(!ToolDescriptor.IsActive)
                {
                    return 0;
                }

                var configSource = Path.Combine(CommonUtils.ArtifactsFolder, "osquery.conf");
                var configDestination = Path.Combine(ToolDescriptor.InstallationPath, "osquery.conf");
                EnsureSourceToDestination(configSource, configDestination);

                var packsSourcePath = Path.Combine(CommonUtils.ArtifactsFolder, "osquery-packs.zip");
                var packsDestinationPath = Path.Combine(ToolDescriptor.InstallationPath, "packs");
                ExtractSourceToDestination(packsSourcePath, packsDestinationPath);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
                return 1;
            }
        }

        public int Remove()
        {
            return UninstallMsi();
        }
    }
}
