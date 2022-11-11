using Common.Utils;
using Serilog;
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

            Log.Logger.Information($"Deceptive Bytes remove exit code={wazuhExitCode}");

            Thread.Sleep(1000);

            var osQueryExitCode = ToolManager.AgentWrappers.OsQueryWrapper.Remove();

            Log.Logger.Information($"Deceptive Bytes remove exit code={osQueryExitCode}");

            Thread.Sleep(1000);

            var sysmonExitCode = ToolManager.AgentWrappers.SysmonWrapper.Remove();

            Log.Logger.Information($"Deceptive Bytes remove exit code={sysmonExitCode}");

        }
    }
}
