using Common.ConfigProvider;
using Common.ServiceHelpers;
using Serilog;
using System;
using System.Threading.Tasks;
using ToolManager;

namespace ToolChecker
{
    internal class Program
    {
        private static async Task Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            Log.Logger.Information($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Log.Logger.Information($"Artifacts: {CommonUtils.LogsFolder}");

            ServiceStatusWatcher.ServiceStatusChanged += (serviceName, status) =>
            {
                Log.Logger.Information($"Service {serviceName} changed status to {status}");
            };

            ServiceStatusWatcher.AddService("IBMPMSVC");
            ServiceStatusWatcher.AddService("Lenovo Instant On");

            Console.ReadLine();
        }

        public static void CheckInstallStatus()
        {
            if (MsiPackageWrapper.GetProductInfoReg("7-Zip", out var pi7zip)) Log.Logger.Information(pi7zip.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("Git", out var piGit)) Log.Logger.Information(piGit.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("osquery", out var piOsq)) Log.Logger.Information(piOsq.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("wazuh", out var piWaz)) Log.Logger.Information(piWaz.ToString());

            if (ServiceHelper.GetServiceInfo("Sysmon64", out var piSym)) Log.Logger.Information(piSym.ToString());
        }
    }
}