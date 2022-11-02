using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public void CaptureEvent(string name, InstallStatus installStatus, RunningStatus runningStatus)
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                var toolEntry = col.FindOne(x => x.Name == name);

                if (toolEntry == null)
                {
                    Logger.Error($"Tool not found: {name}");
                    return;
                }

                toolEntry.InstallStatus = installStatus;
                toolEntry.RunningStatus = runningStatus;
                col.Update(toolEntry);
            }

            var toolStatus = new ToolStatus(name, installStatus, runningStatus);

            Logger.Information($"{toolStatus}");

            var log = new EventLog(Constants.LogGroupName) { Source = Constants.IvsAgentName };

            var eventInstance = new EventInstance(toolStatus.GetHashCode(), 0, EventLogEntryType.Information);

            log.WriteEvent(eventInstance, $"{name} Install: {installStatus}");
        }

        public bool IsAnyError()
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                return col.Exists(x => x.InstallStatus != InstallStatus.Installed || x.RunningStatus != RunningStatus.Running);
            }
        }
    }
}
