using Newtonsoft.Json;

namespace Common.Persistence
{
    public class ToolStatus
    {
        [JsonConstructor]
        public ToolStatus(int groupId, InstallStatus installStatus, RunningStatus runningStatus)
        {
            Group = groupId;
            InstallStatus = installStatus;
            RunningStatus = runningStatus;
        }

        public ToolStatus(long eventId)
        {
            Group = (int) (eventId / 100) * 100;
            InstallStatus = (InstallStatus)((eventId - Group * 100) / 10);
            RunningStatus = (RunningStatus)(eventId - Group * 100 - (int)InstallStatus * 10);
        }

        [JsonProperty("group")]
        public int Group { get; }

        [JsonProperty("installStatus")]
        public InstallStatus InstallStatus { get; }

        [JsonProperty("runningStatus")]
        public RunningStatus RunningStatus { get; }

        public override int GetHashCode()
        {
            var code = Group;
            code += (int)InstallStatus * 10;
            code += (int)RunningStatus;
            return code;
        }

        public override string ToString()
        {
            return $"{ToolGroup.FromId<ToolGroup>(Group).Name} Install: {InstallStatus} Running: {RunningStatus}";
        }
    }    
}
