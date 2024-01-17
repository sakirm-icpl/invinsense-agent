using Serilog;
using Common.Models;

namespace ToolManager
{
    public sealed class InvinsenseManager : ProductManager
    {
        public InvinsenseManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(InvinsenseManager)))
        {
            _logger.Information($"Initializing {nameof(InvinsenseManager)} Manager");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override void PostInstall()
        {
            
        }

        /// <summary>
        /// </summary>
        /// <param name="detail"></param>
        protected override void BeforeUninstall(InstallStatusWithDetail detail)
        {
            
        }
    }
}