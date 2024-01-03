using ToolManager.Models;
using Serilog;

namespace ToolManager
{
    public sealed class WazuhManager : ProductManager
    {
        public WazuhManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(WazuhManager)))
        {
            _logger.Information($"Initializing {nameof(WazuhManager)} Manager");
        }

        public override void PostInstall()
        {
           
        }
    }
}