﻿using Common.ConfigProvider;
using Common.Models;
using Common.Net;
using Common.ServiceHelpers;
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

            ServiceMonitorUtility.ServiceStatusChanged += (serviceName, status) =>
            {
                Log.Logger.Information($"Service {serviceName} changed status to {status}");
            };

            ServiceMonitorUtility.AddService("IBMPMSVC");
            ServiceMonitorUtility.AddService("Lenovo Instant On");

            Console.ReadLine();
        }

        public static void CheckInstallStatus()
        {
            if (MsiPackageWrapper.GetProductInfoReg("7-Zip", out var pi7zip)) Log.Logger.Information(pi7zip.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("Git", out var piGit)) Log.Logger.Information(piGit.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("osquery", out var piOsq)) Log.Logger.Information(piOsq.ToString());

            if (MsiPackageWrapper.GetProductInfoReg("wazuh", out var piWaz)) Log.Logger.Information(piWaz.ToString());

            if (ServiceHelper.GetServiceInfo("Sysmon64", out var piSym)) Log.Logger.Information(piSym.ToString());
        }

        public static async Task Install()
        {
            var client = new ClientService(new HttpClientConfig
            {
                Name = "Invinsense.Server",
                AuthRequired = false,
                TimeOut = 60,
                BaseUrl = "https://65.1.109.28:5001",
                BaseHeaders = new Dictionary<string, string>(),
                ExtraParams = new Dictionary<string, string>()
            });

            Log.Logger.Information(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

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

            foreach (var td in toolDetails)
            {
                Log.Logger.Information($"{td.Key} - {td.Value}");
            }

            var otd = toolDetails[ToolName.OsQuery];
            var om = new OsQueryManager(otd);
            if (om.PreInstall() == 0)
            {
                om.InstallProduct();
            }
            om.PostInstall();

            var sd = toolDetails[ToolName.Sysmon];
            var sm = new SysmonManager(sd);
            if (sm.PreInstall() == 0)
            {
                sm.InstallProduct();
            }
            sm.PostInstall();

            var wd = toolDetails[ToolName.Wazuh];
            var wm = new WazuhManager(wd);
            if (wm.PreInstall() == 0)
            {
                wm.InstallProduct();
            }
            wm.PostInstall();

            Console.WriteLine("DONE. Press any key to exist.");
        }

    }
}