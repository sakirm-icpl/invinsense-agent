using Common.Utils;
using Serilog;
using System;
using ToolManager;

namespace OsQueryChecker
{
    internal class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            /*
            if (!Directory.Exists(@"C:\ProgramData\Infopercept"))
            {
                Log.Logger.Information("Creating directory C:\\ProgramData\\Infopercept");
                Directory.CreateDirectory(@"C:\ProgramData\Infopercept");
            }

            if(!Directory.Exists(@"C:\ProgramData\Infopercept\Artifacts"))
            {
                Log.Logger.Information("Creating directory C:\\ProgramData\\Infopercept\\Artifacts");
                Directory.CreateDirectory(@"C:\ProgramData\Infopercept\Artifacts");
            }
            */

            Console.WriteLine($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Console.WriteLine($"Artifacts: {CommonUtils.LogsFolder}");

            var osQueryWrapper = new OsQueryWrapper();

            var exitCode = osQueryWrapper.EnsureInstall();

            Log.Logger.Information($"Completed.{exitCode}");

            Console.ReadLine();
        }
    }
}
