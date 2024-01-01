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

            var apiResponse = await client.InvokeAsync(HttpMethodNames.Get, $"/api/tools");

            var content = Encoding.UTF8.GetString(apiResponse.Response, 0, apiResponse.Response.Length);

            Dictionary<string, ToolDetail> toolDetails = null;

            try
            {
                if (apiResponse.IsSuccess && apiResponse.Response.Length > 0)
                {
                    toolDetails = JsonConvert.DeserializeObject<Dictionary<string, ToolDetail>>(content, SerializationExtension.DefaultOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP:{IsSuccess} - {Message} : payload: {Payload}", apiResponse.IsSuccess, ex.Message);
                return;
            }

            var osQueryToolDetail = toolDetails[ToolName.OsQuery];

            var osQueryManager = new OsQueryManager(osQueryToolDetail);

            osQueryManager.Preinstall();
        }

    }
}