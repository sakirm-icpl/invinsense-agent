using System.Collections.Generic;

namespace Common.Persistance
{
    public sealed class ToolRepository : BaseRepository<ToolDetail>
    {
        protected override string CollectionName => "tool_details";

        public ToolRepository()
        {
            
        }

        public IEnumerable<ToolDetail> GetTools()
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                return col.FindAll();
            }
        }

        public ToolDetail GetDetail(string name)
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                return col.FindOne(x=>x.Name == name);
            }
        }

        public void CaptureInstallationEvent(string name, InstallStatus toolInstallStatus)
        {
            Logger.Information($"{name}-{toolInstallStatus}");
        }

        public ToolStatus GetToolStatus(long eventId)
        {
            return new ToolStatus
            {
                Name = "WAZUH",
                InstallStatus = InstallStatus.Installed,
                RunningStatus = RunningStatus.Running
            };
        }

        public void CaptureRunningEvent(string name, RunningStatus toolRunningStatus)
        {
            Logger.Information($"{name}-{toolRunningStatus}");
        }

        public bool IsAllOk()
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                return col.Exists(x=> x.InstallStatus != InstallStatus.Installed || x.RunningStatus != RunningStatus.Running);
            }
        }
    }
}
