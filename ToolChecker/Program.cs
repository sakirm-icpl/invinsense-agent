using Common.ConfigProvider;
using Common.Net;
using Common.Persistence;
using Common.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ToolManager;

namespace ToolChecker
{
    internal class Program
    {
        private static async Task Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            Console.WriteLine($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Console.WriteLine($"Artifacts: {CommonUtils.LogsFolder}");

            var client = new ClientService(new HttpClientConfig
            {
                Name = "Invinsense.Server",
                AuthRequired = false,
                TimeOut = 60,
                BaseUrl = "http://localhost:5197",
                BaseHeaders = new Dictionary<string, string>(),
                ExtraParams = new Dictionary<string, string>()
            });

            var apiResponse = await client.InvokeAsync(HttpMethodNames.Get, $"/api/tools/osquery");

            var content = Encoding.UTF8.GetString(apiResponse.Response, 0, apiResponse.Response.Length);

            ToolDetail toolDetail = null;

            try
            {
                if (apiResponse.IsSuccess && apiResponse.Response.Length > 0)
                {
                    toolDetail = JsonConvert.DeserializeObject<ToolDetail>(content, SerializationExtension.DefaultOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP:{IsSuccess} - {Message} : payload: {Payload}", apiResponse.IsSuccess, ex.Message);
            }

            var osQueryManager = new OsQueryManager(toolDetail);

            var success = osQueryManager.GetInstalledVersion(out Version version);

            if (!success)
            {
                Console.WriteLine("OsQuery Error in detecting version.");
                return;
            }

            var requiredVersion = new Version(toolDetail.Version);
            var minimumVersion = new Version(toolDetail.MinVersion);
            var maximumVersion = new Version(toolDetail.MaxVersion);

            if(version == null || version < minimumVersion)
            {
                // Download required files from server.
                Console.WriteLine("OsQuery version is less than minimum version.");

                var downloadUrl = toolDetail.DownloadUrl;

                Console.WriteLine($"Downloading {downloadUrl}");

                var downloader = new FragmentedFileDownloader();

                await downloader.DownloadFileAsync(downloadUrl, Path.Combine(CommonUtils.ArtifactsFolder, "osquery.zip"));


            }
            else if(version > maximumVersion)
            {
                Console.WriteLine("OsQuery version is greater than maximum version.");
            }

            if(version == requiredVersion)
            {
                Console.WriteLine("OsQuery version is equal to required version.");
            }

            Console.WriteLine($"OsQuery version: {version}");           


            /*
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
            */
            Console.ReadLine();
        }

    }
}