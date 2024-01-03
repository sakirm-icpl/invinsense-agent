using ToolManager.Models;
using Serilog;

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
            return base.Preinstall();
        }

        public override int PostInstall()
        {
            return base.PostInstall();
        }
    }
}
