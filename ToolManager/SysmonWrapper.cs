using Serilog;
using Common.ConfigProvider;
using Common.FileHelpers;
using System.IO;
using System;
using Common.Models;

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

        /// <summary>
        /// Copy config and update
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected override void PostInstall()
        {
            var sourceFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);

            var destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            _logger.Information($"Source: {sourceFolder}, Destination: {destinationFolder}");

            //Copy osquery.conf to installation path
            var configSource = Path.Combine(sourceFolder, "sysmonconfig.xml");
            var configDestination = Path.Combine(destinationFolder, "sysmonconfig.xml");
            CommonFileHelpers.EnsureSourceToDestination(configSource, configDestination);

            var exeSource = Path.Combine(destinationFolder, "Sysmon64.exe");

            //configure service
            var p = ProcessExtensions.CreateHiddenProcess(exeSource, $"-c {configDestination}");
            p.Start();
            p.WaitForExit();
        }

        protected override void BeforeUninstall(InstallStatusWithDetail detail)
        {

        }
    }
}
