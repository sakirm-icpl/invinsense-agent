using System.ServiceProcess;

namespace IvsAgent.Monitor
{
    public static class ServiceHelper
    {
        public static string GetServiceStatus(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            return sc.Status.ToString();
        }

        public static void SendCommand(string serviceName, int command)
        {
            var service = new ServiceController(serviceName);
            service.ExecuteCommand(command);
        }
    }
}
