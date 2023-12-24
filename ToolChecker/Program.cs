using System;
using Common.ConfigProvider;
using Serilog;

namespace ToolChecker
{
    internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            Console.WriteLine($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Console.WriteLine($"Artifacts: {CommonUtils.LogsFolder}");

            Console.ReadLine();
        }
    }
}