using Common.ConfigProvider;
using Serilog;
using System;

namespace ToolChecker
{
    internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            Console.WriteLine($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Console.WriteLine($"Artifacts: {CommonUtils.LogsFolder}");

            var filePaths = new[]
            {
                @"C:\code\invinsense-agent\artifacts\osquery\osquery-5.5.1.msi",
                @"C:\code\invinsense-agent\artifacts\osquery\osquery-5.8.2.msi",
                @"C:\code\invinsense-agent\artifacts\osquery\osquery-5.10.2.msi",
                @"C:\code\invinsense-agent\artifacts\wazuh\wazuh-agent-4.4.1-1.msi",
                @"C:\code\invinsense-agent\artifacts\sysmon\Sysmon64.exe"
            };

            foreach (var file in filePaths)
            {
                if (MsiWrapper.MsiPackageWrapper.GetMsiVersion(file, out Version version))
                {
                    Console.WriteLine($"{version} - {file}");
                }
            }

            Console.ReadLine();
        }

    }
}