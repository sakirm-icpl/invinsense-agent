using System.Diagnostics;

namespace Common
{
    
    public sealed class TrackingEvent
    {
        public int Id { get; }

        public string ServiceName { get; set; }

        public string SvcName { get; set; }

        public string Message { get; set; }

        public EventLogEntryType EventType { get; set; }

        [System.Text.Json.Serialization.JsonConstructor]
        public TrackingEvent(int id, string name, string svcName, EventLogEntryType eventType, string message) => (Id, ServiceName, SvcName, EventType, Message) = (id, name, svcName, eventType, message);

        public override int GetHashCode() => Id;

        public override string ToString() => $"{ServiceName} - {Message}";
    }
}
