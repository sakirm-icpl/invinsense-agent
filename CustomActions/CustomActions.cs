using Microsoft.Deployment.WindowsInstaller;
using System.Linq;
using System.ServiceProcess;

namespace ToolManager
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckSessionParameters(Session session)
        {
            session.Log("Preparing Install. Checking required parameters.");

            if(string.IsNullOrEmpty(session["WAZUH_MANAGER"]))
            {
                session.Log($"Required parameter '{session["WAZUH_MANAGER"]}' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["WAZUH_REGISTRATION_SERVER"]))
            {
                session.Log($"Required parameter '{session["WAZUH_REGISTRATION_SERVER"]}' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["WAZUH_AGENT_GROUP"]))
            {
                session.Log($"Required parameter '{session["WAZUH_AGENT_GROUP"]}' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["DBYTES_SERVER"]))
            {
                session.Log($"Required parameter '{session["DBYTES_SERVER"]}' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["DBYTES_APIKEY"]))
            {
                session.Log($"Required parameter '{session["DBYTES_APIKEY"]}' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StopAgentService(Session session)
        {
            session.Log("Trying to stop agent service...");

            if (ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals("IvsAgent")))
            {
                session.Log("Stopping Invinsense service");
                var service = new ServiceController("IvsAgent");
                if(service.Status == ServiceControllerStatus.Running)
                {
                    service.ExecuteCommand(130);
                    session.Log("Stopping Invinsense service. Wait for Status Stopped");
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }

                session.Log($"Invinsense service stauts: {service.Status}");
            }
            else
            {
                session.Log("Invinsense service does not exists... No action required...");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult VerifyRemoveKey(Session session)
        {
            session.Log("Preparing Uninstall. Checking required parameters...");

            if (session["UNINSTALL_KEY"] != "ICPL_2023")
            {
                session.Log("Uninstall key not supplied. Uninstall process failed...");
                return ActionResult.Failure;
            }

            session.Log("Uninstall key verified. Proceeding further...");
            return ActionResult.Success;
        }
    }
}
