using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System;
using System.ServiceProcess;
using Common.RegistryHelpers;
using Common.Net;
using System.Threading.Tasks;
using Serilog;
using Common.ConfigProvider;
using Common;
using Common.Models;
using Newtonsoft.Json;
using Common.Utils;
using ToolManager;

namespace IvsUpgrade
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.File(CommonUtils.GetLogFilePath("IvsUpgrade.log"), rollOnFileSizeLimit: true, retainedFileCountLimit: 5, fileSizeLimitBytes: 30000, rollingInterval: RollingInterval.Day)
                   .WriteTo.Console()
                   .CreateLogger();

            var logger = Log.ForContext<Program>();

            try
            {
                string serviceName = "IvsAgent";

                ServiceController[] services = ServiceController.GetServices();
                ServiceController sc = services.FirstOrDefault(s => s.ServiceName == serviceName);
                if (sc != null)
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        Thread.Sleep(5000);
                    }
                }
                else
                {
                    logger.Information("The service has been stopped early..");
                }

                var apiUrl = WinRegistryHelper.GetPropertyByName(Constants.CompanyName, "ApiUrl");

                if (string.IsNullOrEmpty(apiUrl))
                {
                    if(args.Length == 0)
                    {
                        logger.Error("ApiUrl is not set in registry");
                        return;
                    }
                    apiUrl = args[0];
                }
               
                var client = new ClientService(new HttpClientConfig
                {
                    Name = "API",
                    AuthRequired = false,
                    TimeOut = 60,
                    BaseUrl = apiUrl,
                    BaseHeaders = new Dictionary<string, string>(),
                    ExtraParams = new Dictionary<string, string>()
                });

                logger.Information(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

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
                    logger.Error("HTTP:{IsSuccess} - {Message} : payload: {Payload}", apiResponse.IsSuccess, ex.Message);
                    return;
                }

                foreach (var td in toolDetails)
                {
                    logger.Information($"{td.Key} - {td.Value}");
                }

                logger.Information("Agent Upgrade is ready");

                var td = toolDetails[ToolName.Invinsense];
                var tm = new InvinsenseManager(td);
                tm.Ensure();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
