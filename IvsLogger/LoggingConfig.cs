using Common.Utils;
using Serilog;

namespace IvsAgent
{
    public static class LoggingConfig
    {
        public static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.File(new CustomJsonFormatter(), CommonUtils.DataFolder + "\\ivsagent.json", rollOnFileSizeLimit: false, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, fileSizeLimitBytes: 10000)
                   .CreateLogger();
        }
    }
}
