namespace Common
{
    public sealed class TrackingEvent
    {
        public int Id { get; }

        public string ServiceName { get; set; }

        public string SvcName { get; set; }

        public string Message { get; set; }

        public TrackingEvent(int id, string name, string svcName, string message) => (Id, ServiceName, SvcName, Message) = (id, name, svcName, message);

        public override int GetHashCode() => Id;

        public override string ToString() => $"{ServiceName} - {Message}";
    }
}
