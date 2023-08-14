using System.IO;
using System;
using System.Linq;
using System.ServiceProcess;
using Common.Utils;
using WixToolset.Dtf.WindowsInstaller;

namespace ToolManager
{
    /// <summary>
    /// https://wixtoolset.org/docs/v3/xsd/wix/installexecutesequence/
    /// </summary>
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult RemoveOldDataFiles(Session session)
        {
            session.Log("RemoveOldDataFiles");
            session.Log("We do not need to remove old data files.");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CheckSessionParameters(Session session)
        {
            session.Log("Preparing Install. Checking required parameters.");

            if (string.IsNullOrEmpty(session["WAZUH_MANAGER"]))
            {
                session.Log($"Required parameter 'WAZUH_MANAGER' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["WAZUH_REGISTRATION_SERVER"]))
            {
                session.Log($"Required parameter 'WAZUH_REGISTRATION_SERVER' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["WAZUH_AGENT_GROUP"]))
            {
                session.Log($"Required parameter 'WAZUH_AGENT_GROUP' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["WAZUH_REGISTRATION_TYPE"]))
            {
                session.Log($"Required parameter 'WAZUH_REGISTRATION_TYPE' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            var authType = session["WAZUH_REGISTRATION_TYPE"];

            if(!(authType == "NONE" || authType == "PASSWORD" || authType == "CERTIFICATE"))
            {
                session.Log($"'WAZUH_REGISTRATION_TYPE' value is invalid. Expected: NONE | PASSWORD | CERTIFICATE");
                return ActionResult.Failure;
            }

            if (authType == "PASSWORD" && string.IsNullOrEmpty(session["WAZUH_REGISTRATION_PASSWORD"]))
            {
                session.Log($"Required parameter 'WAZUH_REGISTRATION_PASSWORD' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (authType == "CERTIFICATE" && (string.IsNullOrEmpty(session["WAZUH_REGISTRATION_CERTIFICATE"]) || string.IsNullOrEmpty(session["WAZUH_REGISTRATION_KEY"])))
            {
                session.Log($"Required parameter 'WAZUH_REGISTRATION_CERTIFICATE' or 'WAZUH_REGISTRATION_KEY' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            var skipEndpointDeception = session["SKIP_ENDPOINT_DECEPTION"];
            if (skipEndpointDeception == "Y" || skipEndpointDeception == "y")
            {
                return ActionResult.Success;
            }

            if (string.IsNullOrEmpty(session["DBYTES_SERVER"]))
            {
                session.Log($"Required parameter 'DBYTES_SERVER' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["DBYTES_APIKEY"]))
            {
                session.Log($"Required parameter 'DBYTES_APIKEY' is missing. Installer will exit.");
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
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.ExecuteCommand(130);
                    session.Log("Stopping Invinsense service. Wait for Status Stopped");
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }

                session.Log($"Invinsense service status: {service.Status}");
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
                session.Log($"Uninstall key {session["UNINSTALL_KEY"]} not valid. Uninstall process failed...");
                return ActionResult.Failure;
            }

            session.Log("Uninstall key verified. Proceeding further...");
            return ActionResult.Success;
        }
    }
}
