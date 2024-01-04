using ToolManager.Models;
using Serilog;
using Common.ConfigProvider;
using Common.FileHelpers;
using System.IO;
using System;
using Common.RegistryHelpers;

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
                CopyConfig();  
            }

            return status;
        }

        public override int PostInstall()
        {
            var lastUpdate = WinRegistryHelper.GetPropertyByName("Infopercept", "sysmon_last_update");

            var lastUpdateTime = lastUpdate == null ? DateTime.MinValue : DateTime.Parse(lastUpdate);

            if (_toolDetail.UpdatedOn <= lastUpdateTime)
            {
                _logger.Information("Sysmon config is up to date.");
                return base.PostInstall();   
            }

            CopyConfig();
            WinRegistryHelper.SetPropertyByName("Infopercept", "sysmon_last_update", DateTime.Now.ToString());
            return base.PostInstall();
        }

        private void CopyConfig()
        {
            var sourceFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);

            var destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            _logger.Information($"Source: {sourceFolder}, Destination: {destinationFolder}");

            //Copy osquery.conf to installation path
            var configSource = Path.Combine(sourceFolder, "sysmonconfig.xml");
            var configDestination = Path.Combine(destinationFolder, "sysmonconfig.xml");
            CommonFileHelpers.EnsureSourceToDestination(configSource, configDestination);
        }
    }
}
