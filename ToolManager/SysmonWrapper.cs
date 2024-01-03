using ToolManager.Models;
using Serilog;
using Common.ConfigProvider;
using Common.FileHelpers;
using System.IO;
using System;

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

        public override int Preinstall()
        {
            var status = base.Preinstall();

            if(status == 0)
            {
                var sourceFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);
                var destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Infopercept");

                _logger.Information($"Source: {sourceFolder}, Destination: {destinationFolder}");

                //Copy osquery.conf to installation path
                var configSource = Path.Combine(sourceFolder, "sysmonconfig.xml");
                var configDestination = Path.Combine(destinationFolder, "sysmonconfig.xml");
                CommonFileHelpers.EnsureSourceToDestination(configSource, configDestination);
            }

            return status;
        }

        public override int PostInstall()
        {
            return base.PostInstall();
        }
    }
}
