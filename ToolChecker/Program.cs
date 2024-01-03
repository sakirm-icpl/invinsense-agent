using Common.ConfigProvider;
using Common.Net;
using ToolManager.Models;
using Common.RegistryHelpers;
using Common.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
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

            Log.Logger.Information($"Artifacts: {CommonUtils.ArtifactsFolder}");
            Log.Logger.Information($"Artifacts: {CommonUtils.LogsFolder}");

            if (ProductManager.GetServiceInfo("Sysmon64", out var sysmonInfo)) Console.WriteLine(sysmonInfo);

            if (ProductManager.GetProductInfoReg("7-Zip", out var _7zip)) Console.WriteLine(_7zip);

            if (ProductManager.GetProductInfoReg("Git", out var git)) Console.WriteLine(git);

            if (ProductManager.GetProductInfoReg("osquery", out var osquery)) Console.WriteLine(osquery);

            var client = new ClientService(new HttpClientConfig
            {
                Name = "Invinsense.Server",
                AuthRequired = false,
                TimeOut = 60,
                BaseUrl = "https://65.1.109.28:5001",
                BaseHeaders = new Dictionary<string, string>(),
                ExtraParams = new Dictionary<string, string>()
            });

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

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
                Log.Logger.Error("HTTP:{IsSuccess} - {Message} : payload: {Payload}", apiResponse.IsSuccess, ex.Message);
                return;
            }

            var otd = toolDetails[ToolName.OsQuery];
            var om = new OsQueryManager(otd);

            var status = om.Preinstall();

            if(status == 0)
            {
                om.InstallProduct();
            }

            var lastUpdate = WinRegistryHelper.GetPropertyByName("Infopercept", "osquery_last_update");

            var lastUpdateTime = lastUpdate == null ? DateTime.MinValue : DateTime.Parse(lastUpdate);

            if(otd.UpdatedOn >= lastUpdateTime)
            {
                om.PostInstall();
                WinRegistryHelper.SetPropertyByName("Infopercept", "osquery_last_update", DateTime.Now.ToString());
            }

            var sd = toolDetails[ToolName.Sysmon];
            var sm = new SysmonManager(sd);
            status = sm.Preinstall();
            if (status == 0)
            {
                sm.InstallProduct();
            }

            lastUpdate = WinRegistryHelper.GetPropertyByName("Infopercept", "sysmon_last_update");

            lastUpdateTime = lastUpdate == null ? DateTime.MinValue : DateTime.Parse(lastUpdate);

            if (sd.UpdatedOn >= lastUpdateTime)
            {
                sm.PostInstall();
                WinRegistryHelper.SetPropertyByName("Infopercept", "sysmon_last_update", DateTime.Now.ToString());
            }

            /*
            var wd = toolDetails[ToolName.Wazuh];
            var wm = new WazuhManager(wd);
            status = wm.Preinstall();
            if (status == 0)
            {
                wm.Install();
            }

            lastUpdate = WinRegistryHelper.GetPropertyByName("Infopercept", "wazuh_last_update");

            lastUpdateTime = lastUpdate == null ? DateTime.MinValue : DateTime.Parse(lastUpdate);

            if (otd.UpdatedOn >= lastUpdateTime)
            {
                wm.PostInstall();
                WinRegistryHelper.SetPropertyByName("Infopercept", "wazuh_last_update", DateTime.Now.ToString());
            }
            */

            Console.ReadLine();
        }

    }
}