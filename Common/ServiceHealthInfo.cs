namespace Common
{
    public class ServiceHealthInfo
    {
        public string ServiceName { get; }
        
        public string SvcName { get; }
        
        public ServiceStatus Status { get; set; }

        public ServiceHealthInfo(string serviceName, string svcName, ServiceStatus status)
        {
            ServiceName = serviceName;
            SvcName = svcName;
            Status = status;
        }
    }
}
