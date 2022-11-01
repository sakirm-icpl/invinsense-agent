using System.Collections.Generic;
using System.Diagnostics;

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
                return col.FindOne(x => x.Name == name);
            }
        }

        public void CaptureInstallationEvent(string name, InstallStatus toolInstallStatus)
        {
            Logger.Information($"{name}-{toolInstallStatus}");

            var log = new EventLog(Constants.LogGroupName) { Source = Constants.IvsAgentName };

            var eventInstance = new EventInstance(100, 1, EventLogEntryType.Information);

            log.WriteEvent(eventInstance, $"{name} Install: {toolInstallStatus}");
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
                return col.Exists(x => x.InstallStatus != InstallStatus.Installed || x.RunningStatus != RunningStatus.Running);
            }
        }
    }
}
