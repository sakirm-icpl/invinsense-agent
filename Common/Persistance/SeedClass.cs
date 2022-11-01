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
                var col = db.GetCollection<ToolDetail>("tool_details");

                // Create unique index in Name field
                col.EnsureIndex(x => x.Name, true);

                // Create your new customer instance
                var osQueryTool = new ToolDetail
                {
                    Name = "OSQUERY",
                    IsInstalled = false,
                    IsMsi = true,
                    InstallScript = "osquery-5.5.1.msi",
                    UnInstallScript = "",
                    AppName = "osquery",
                    IsService = true,

                    IsActive = true
                };

                var sysmonTool = new ToolDetail
                {
                    Name = "SYSMON",
                    IsInstalled = false,
                    IsMsi = false,
                    InstallScript = "Sysmon64 -i",
                    UnInstallScript = "",
                    AppName = "osquery",
                    IsService = true,
                    IsActive = true
                };

                var toolDetails = new List<ToolDetail> { osQueryTool, sysmonTool };

                // Insert new customer document (Id will be auto-incremented)
                col.InsertBulk(toolDetails);
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
