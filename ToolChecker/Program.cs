using Common.ConfigProvider;
using Common.ServiceHelpers;
using ConsoleMenu;
using Serilog;
using System;
using ToolManager;

namespace ToolChecker
{
    internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            var logger = Log.Logger.ForContext(typeof(Program));

            logger.Information($"Artifacts: {CommonUtils.ArtifactsFolder}");
            logger.Information($"Artifacts: {CommonUtils.LogsFolder}");

            ServiceStatusWatcher.ServiceStatusChanged += (serviceName, status) =>
            {
                logger.Information($"Service {serviceName} changed status to {status}");
            };

            var consoleMenu = new ConsoleMenuUtility();
            consoleMenu.DisplayMenuAndHandleInput();
        }

        [ConsoleOption(1, "Check Install Status of 7zip, Git, OSQuery, Sysmon and Wazuh")]
        public static void CheckInstallStatus()
        {
            var logger = Log.Logger.ForContext(typeof(Program));

            if (MsiPackageWrapper.GetProductInfoReg("7-Zip", out var pi7zip)) logger.Information(pi7zip.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("Git", out var piGit)) logger.Information(piGit.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("osquery", out var piOsq)) logger.Information(piOsq.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("Wazuh Agent", out var piWaz)) logger.Information(piWaz.ToString());

            if (ServiceHelper.GetServiceInfo("Sysmon64", out var piSym)) logger.Information(piSym.ToString());
        }

        [ConsoleOption(2, "Start monitoring services IBMPMSVC and 'Lenovo Instant On'")]
        public static void MonitorSampleServices()
        {
            ServiceStatusWatcher.AddService("IBMPMSVC");
            ServiceStatusWatcher.AddService("Lenovo Instant On");
        }

        [ConsoleOption(3, "Monitor antivirus status")]
        public static void MonitorAntivirus()
        {
            var logger = Log.Logger.ForContext(typeof(Program));
            
            Common.AvHelper.AvStatusWatcher.Instance.AvStatusChanged += (sender, args) =>
            {
                logger.Information($"Antivirus status changed to {args.RunningStatus}");
            };

            Common.AvHelper.AvStatusWatcher.Instance.StartMonitoring();
        }

        [ConsoleOption(4, "Install Tools from server")]
        public static void RemoveSampleServices()
        {
            CheckRequiredTools.Install("https://65.1.109.28:5001").Wait();
        }
    }
}