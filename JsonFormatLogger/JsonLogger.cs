using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFormatLogger
{
    public class JsonLogger
    {
        public static string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Infopercept");
        public static ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(new JsonFormatter(), DataFolder + "\\IvsAgent.json", rollOnFileSizeLimit: false, fileSizeLimitBytes: 100000)
            .CreateLogger();
        }
    }
}
