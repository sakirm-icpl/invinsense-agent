using Common.Utils;
using Serilog;
using System.Collections.Generic;
using System.Management;
using System.Threading;

namespace IvsUninstall
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(CommonUtils.GetAbsoletePath("ivsuninstall.log"), rollingInterval: RollingInterval.Day)
               .WriteTo.Console()
               .CreateLogger();

            Log.Logger.Information("Uninstalling Invinsense 3.0 components");

            Thread.Sleep(1000);

            Log.Logger.Information("Uninstalling Deceptive Bytes...");

            var dBytesExitCode = ToolManager.AgentWrappers.DBytesWrapper.Remove();

            Log.Logger.Information($"Deceptive Bytes remove exit code={dBytesExitCode}");

            Thread.Sleep(1000);

            var wazuhExitCode = ToolManager.AgentWrappers.WazuhWrapper.Remove();

            Log.Logger.Information($"Wazuh Bytes remove exit code={wazuhExitCode}");

            Thread.Sleep(1000);

            var osQueryExitCode = ToolManager.AgentWrappers.OsQueryWrapper.Remove();

            Log.Logger.Information($"OSQUERY Bytes remove exit code={osQueryExitCode}");

            Thread.Sleep(1000);

            var sysmonExitCode = ToolManager.AgentWrappers.SysmonWrapper.Remove();

            Log.Logger.Information($"SYSMON Bytes remove exit code={sysmonExitCode}");

        }

        private static List<string> ListPrograms()
        {
            List<string> programs = new List<string>();

            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        //more properties:
                        //http://msdn.microsoft.com/en-us/library/windows/desktop/aa394378(v=vs.85).aspx
                        programs.Add(mo["Name"].ToString());

                    }
                    catch 
                    {
                        //this program may not have a name property
                    }
                }

                return programs;

            }
            catch
            {
                return programs;
            }
        }

        private static bool UninstallProgram(string ProgramName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            object hr = mo.InvokeMethod("Uninstall", null);
                            return (bool)hr;
                        }
                    }
                    catch
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }

                //was not found...
                return false;

            }
            catch
            {
                return false;
            }
        }
    }
}
