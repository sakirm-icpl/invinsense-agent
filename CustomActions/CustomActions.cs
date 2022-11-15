using Microsoft.Deployment.WindowsInstaller;

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
        public static ActionResult RemoveTools(Session session)
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
