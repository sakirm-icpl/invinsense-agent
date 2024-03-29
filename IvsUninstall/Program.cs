﻿using Serilog;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using ToolManager;
using System.DirectoryServices.AccountManagement;
using Common.ConfigProvider;
using Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Common.Net;
using System.Threading.Tasks;
using Common.RegistryHelpers;
using Common.Utils;
using Common;

namespace IvsUninstall
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.File(CommonUtils.GetLogFilePath("IvsUninstall.log"), rollOnFileSizeLimit: true, retainedFileCountLimit: 5, fileSizeLimitBytes: 30000, rollingInterval: RollingInterval.Day)
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
                    if (args.Length == 0)
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

                var otd = toolDetails[ToolName.OsQuery];
                var om = new OsQueryManager(otd);
                om.Remove();

                var sd = toolDetails[ToolName.Sysmon];
                var sm = new SysmonManager(sd);
                sm.Remove();

                var wd = toolDetails[ToolName.Wazuh];
                var wm = new WazuhManager(wd);
                wm.Remove();

                var id = toolDetails[ToolName.Invinsense];
                var im = new InvinsenseManager(id);
                im.Remove();

                logger.Information("All components are removed.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            //Removing maintenance
            string username = "maintenance";
            using (PrincipalContext pc = new PrincipalContext(ContextType.Machine))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(pc, username);
                if (user != null)
                {
                    logger.Information("Removing maintenance user");
                    user.Delete();
                    logger.Information("Removing fake file");
                    var fakeFilePath = Path.Combine(@"C:\Users", "Users.txt");
                    if (File.Exists(fakeFilePath))
                    {
                        File.Delete(fakeFilePath);
                    }
                }
            }
        }
    }
}
