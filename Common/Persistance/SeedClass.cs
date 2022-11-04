using Common.Utils;
using LiteDB;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Common.Persistance
{
    public static class SeedClass
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(SeedClass));

        public static void SeedData()
        {
            var path = CommonUtils.DbPath;
            if (File.Exists(path))
            {
                Logger.Information("Db already exists");
                return;
            }

            Logger.Information("Db does not exists. Creating new...");

            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(path))
            {
                // Get customer collection
                var toolsCollection = db.GetCollection<ToolDetail>("tool_details", BsonAutoId.Int32);

                // Create unique index in Name field
                toolsCollection.EnsureIndex(x => x.Name, true);

                // Create your new customer instance
                var wazuhTool = new ToolDetail
                {
                    Name = ToolName.Wazuuh,
                    IsMsi = true,
                    SetupFileName = "wazuh-agent-4.3.9-1",
                    InstallArgs = "/I \"{msiPath}\" /QN /l*vx \"{LOG_PATH}\" ACCEPTEULA=1 ALLUSERS=1 WAZUH_MANAGER=\"{WAZUH_MANAGER}\" WAZUH_REGISTRATION_SERVER=\"{WAZUH_REGISTRATION_SERVER}\"",
                    UpdateArgs = "",
                    UninstallArgs = "/X \"{msiPath}\" /QN /l*vx \"{LOG_PATH}\" /QN",
                    AppName = "WazuhSvc",
                    IsWin64 = false,
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var dBytesTool = new ToolDetail
                {
                    Name = ToolName.Dbytes,
                    IsMsi = true,
                    SetupFileName = "DeceptiveBytes.EPS.x64",
                    InstallArgs = "/I \"{msiPath}\" /l*vx \"{LOG_PATH}\" /QN ALLUSERS=1 /norestart ServerAddress=\"{DBYTES_SERVER}\" ApiKey=\"{DBYTES_APIKEY}\"",
                    UpdateArgs = "",
                    UninstallArgs = "/X \"{msiPath}\" /l*vx \"{LOG_PATH}\" /QN",
                    AppName = "DBytesService",
                    IsWin64 = true,
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var osQueryTool = new ToolDetail
                {
                    Name = ToolName.OsQuery,
                    IsMsi = true,
                    SetupFileName = "osquery-5.5.1.msi",
                    InstallArgs = "/I \"{msiPath}\" /QN /l*vx \"{LOG_PATH}\" ACCEPTEULA=1 ALLUSERS=1",
                    UpdateArgs = "",
                    UninstallArgs = "/X \"{msiPath}\" /QN /l*vx \"{LOG_PATH}\" /QN",
                    AppName = "osqueryd",
                    IsWin64 = true,
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var sysmonTool = new ToolDetail
                {
                    Name = ToolName.Sysmon,
                    IsMsi = false,
                    SetupFileName = "Sysmon64.exe",
                    InstallArgs = "-accepteula -i",
                    UpdateArgs = "",
                    UninstallArgs = "-u force",
                    AppName = "Sysmon64",
                    IsWin64 = true,
                    IsService = true,
                    InstallStatus= InstallStatus.NotFound,
                    RunningStatus= RunningStatus.NotFound
                };

                var avTool = new ToolDetail
                {
                    Name = ToolName.Av,
                    IsMsi = false,
                    InstallArgs = "",
                    UpdateArgs = "",
                    UninstallArgs = "",
                    AppName = "Windows Defender",
                    IsWin64 = true,
                    IsService = false,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var ltmTool = new ToolDetail
                {
                    Name = ToolName.Lmp,
                    IsMsi = true,
                    SetupFileName = "invinsetup.msi", 
                    InstallArgs = "",
                    UpdateArgs = "",
                    UninstallArgs = "",
                    AppName = "IvsAgent",
                    IsWin64 = true,
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var toolDetails = new List<ToolDetail> { wazuhTool, dBytesTool, osQueryTool, sysmonTool, avTool, ltmTool };

                // Insert new customer document (Id will be auto-incremented)
                toolsCollection.InsertBulk(toolDetails);
            }
        }

        public static void ListTools()
        {
            var repo = new ToolRepository();

            foreach (var item in repo.GetTools())
            {
                Logger.Information($"{item.Id}, {item.Name}{Environment.NewLine}");
            }
        }
    }
}
