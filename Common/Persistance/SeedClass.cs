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
                    InstallScript = "",
                    UnInstallScript = "",
                    AppName = "wazuh",
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var dBytesTool = new ToolDetail
                {
                    Name = ToolName.Dbytes,
                    IsMsi = true,
                    InstallScript = "",
                    UnInstallScript = "",
                    AppName = "",
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var osQueryTool = new ToolDetail
                {
                    Name = ToolName.OsQuery,
                    IsMsi = true,
                    InstallScript = "osquery-5.5.1.msi",
                    UnInstallScript = "",
                    AppName = "osqueryd",
                    IsService = true,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var sysmonTool = new ToolDetail
                {
                    Name = ToolName.Sysmon,
                    IsMsi = false,
                    InstallScript = "Sysmon64 -i",
                    UnInstallScript = "",
                    AppName = "Sysmon64",
                    IsService = true,
                    InstallStatus= InstallStatus.NotFound,
                    RunningStatus= RunningStatus.NotFound
                };

                var avTool = new ToolDetail
                {
                    Name = ToolName.Av,
                    IsMsi = false,
                    InstallScript = "",
                    UnInstallScript = "",
                    AppName = "",
                    IsService = false,
                    InstallStatus = InstallStatus.NotFound,
                    RunningStatus = RunningStatus.NotFound
                };

                var ltmTool = new ToolDetail
                {
                    Name = ToolName.Lmp,
                    IsMsi = false,
                    InstallScript = "",
                    UnInstallScript = "",
                    AppName = "IvsAgent",
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
