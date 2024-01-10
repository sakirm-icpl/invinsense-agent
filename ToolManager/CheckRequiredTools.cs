using Common.Models;
using Common.Net;
using Common.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ToolManager
{
    public static class CheckRequiredTools
    {
        private static readonly ILogger logger = Log.Logger.ForContext(typeof(CheckRequiredTools));
        public static async Task Install(string apiUrl)
        {
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

            var otd = toolDetails[ToolName.OsQuery];
            var om = new OsQueryManager(otd);
            om.Ensure();

            var sd = toolDetails[ToolName.Sysmon];
            var sm = new SysmonManager(sd);
            sm.Ensure();

            var wd = toolDetails[ToolName.Wazuh];
            var wm = new WazuhManager(wd);
            wm.Ensure();

            logger.Information("DONE");
        }
    }
}
