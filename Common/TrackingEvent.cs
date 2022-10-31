using System.Diagnostics;

namespace Common
{
    
    public sealed class TrackingEvent
    {
        public int Id { get; }

        public string ServiceName { get; }

        public string SvcName { get; }

        public string Message { get; }

        public EventLogEntryType EventType { get; }

        [Newtonsoft.Json.JsonConstructor]
        public TrackingEvent(int id, string serviceName, string svcName, EventLogEntryType eventType, string message) => (Id, ServiceName, SvcName, EventType, Message) = (id, serviceName, svcName, eventType, message);

        public override int GetHashCode() => Id;

        public override string ToString() => $"{ServiceName} - {Message}";
    }
}
