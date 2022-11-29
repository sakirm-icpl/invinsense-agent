using Common.Utils;
using Serilog;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using ToolManager;
using ToolManager.MsiWrapper;

namespace IvsUninstall
{
    internal class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(CommonUtils.DataFolder + "\\ivsuninstall.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
               .WriteTo.Console()
               .CreateLogger();

            Log.Logger.Information("Uninstalling Invinsense 3.0 components");

            var listPrograms = MoWrapper.ListPrograms();

            foreach (var item in listPrograms)
            {
                Log.Logger.Information($"Program: {item}");
            }

            Thread.Sleep(1000);


            if (ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals("IvsAgent")))
            {
                Log.Logger.Information("Stopping Invinsense service");
                var service = new ServiceController("IvsAgent");
                service.ExecuteCommand(130);
                Thread.Sleep(2000);
            }
            else
            {
                Log.Logger.Information("Invinsense service does not exists...");
            }

            Log.Logger.Information("Uninstalling Deceptive Bytes...");

            var dBytesExitCode = ToolManager.AgentWrappers.DBytesWrapper.Remove();

            Log.Logger.Information($"Deceptive Bytes remove exit code={dBytesExitCode}");

            Thread.Sleep(1000);

            var wazuhExitCode = ToolManager.AgentWrappers.WazuhWrapper.Remove();

            Log.Logger.Information($"Wazuh remove exit code={wazuhExitCode}");

            Thread.Sleep(1000);

            var osQueryExitCode = ToolManager.AgentWrappers.OsQueryWrapper.Remove();

            Log.Logger.Information($"OSQUERY remove exit code={osQueryExitCode}");

            Thread.Sleep(1000);

            var sysmonExitCode = ToolManager.AgentWrappers.SysmonWrapper.Remove();

            Log.Logger.Information($"SYSMON remove exit code={sysmonExitCode}");

            //Removing Agent
            try
            {
                if (!MsiPackageWrapper.IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    Log.Logger.Information("MSI Installer is not free.");
                    return;
                }

                Log.Logger.Information("Agent Uninstallation is ready");

                var logPath = CommonUtils.DataFolder + "\\agentUninstall.log";

                var status = MsiPackageWrapper.Uninstall("Invinsense", logPath, "UNINSTALL_KEY=\"ICPL_2023\"");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }
        }
    }
}
