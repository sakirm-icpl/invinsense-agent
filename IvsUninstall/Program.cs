using Common.Utils;
using Serilog;
using System;
using System.IO;
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
               .WriteTo.File(CommonUtils.DataFolder + "\\ivsuninstall.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
               .WriteTo.Console()
               .CreateLogger();

            Log.Logger.Information("Uninstalling Invinsense 3.0 components");

            var listPrograms = MoWrapper.ListPrograms();

            foreach (var item in listPrograms)
            {
                Log.Logger.Information($"Program: {item}");
            }

            Thread.Sleep(2000);


            if (ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals("IvsAgent")))
            {
                Log.Logger.Information("Stopping Invinsense service");
                var service = new ServiceController("IvsAgent");
                service.ExecuteCommand(130);
                Thread.Sleep(4000);
            }
            else
            {
                Log.Logger.Information("Invinsense service does not exists...");
            }

            #region Deceptive Bytes - Endpoint Deception
            Log.Logger.Information("Uninstalling Deceptive Bytes...");

            var dBytesExitCode = ToolManager.AgentWrappers.DBytesWrapper.Remove();

            Log.Logger.Information($"Deceptive Bytes remove exit code={dBytesExitCode}");

            Thread.Sleep(3000);

            #endregion

            #region OSQUERY

            Log.Logger.Information("Uninstalling OsQuery...");

            var osQueryExitCode = ToolManager.AgentWrappers.OsQueryWrapper.Remove();

            Log.Logger.Information($"OSQUERY remove exit code={osQueryExitCode}");

            Thread.Sleep(3000);

            //TODO:Removing folder : C:\Program Files\osquery
            var osqueryPath = "C:\\Program Files\\osquery";
            if (Directory.Exists(osqueryPath)) Directory.Delete(osqueryPath, true);

            Thread.Sleep(1000);

            #endregion

            #region WAZUH

            Log.Logger.Information("Uninstalling Wazuh...");

            var wazuhExitCode = ToolManager.AgentWrappers.WazuhWrapper.Remove();

            Log.Logger.Information($"Wazuh remove exit code={wazuhExitCode}");

            Thread.Sleep(3000);

            //TODO:Removing folder : C:\\Program Files (x86)\\ossec
            var wazuhPath = "C:\\Program Files (x86)\\ossec";
            if (Directory.Exists(wazuhPath)) Directory.Delete(wazuhPath, true);

            Thread.Sleep(1000);

            #endregion

            #region SYSMON

            Log.Logger.Information("Uninstalling Sysmon...");

            var sysmonExitCode = ToolManager.AgentWrappers.SysmonWrapper.Remove();

            Log.Logger.Information($"SYSMON remove exit code={sysmonExitCode}");

            Thread.Sleep(3000);

            #endregion

            //Removing Agent with uninstall key
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
