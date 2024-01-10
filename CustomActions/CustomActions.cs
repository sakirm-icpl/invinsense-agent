﻿using System.Linq;
using System.ServiceProcess;
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

            if (string.IsNullOrEmpty(session["API_BASE"]))
            {
                session.Log($"Required parameter 'API_BASE' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["EDR_SERVER"]))
            {
                session.Log($"Required parameter 'EDR_SERVER' is missing. Installer will exit.");
                return ActionResult.Failure;
            }

            if (string.IsNullOrEmpty(session["AGENT_GROUPS"]))
            {
                session.Log($"Required parameter 'AGENT_GROUPS' is missing. Installer will exit.");
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

            if (session["UNINSTALL_KEY"] != "ICPL_2024")
            {
                session.Log($"Uninstall key {session["UNINSTALL_KEY"]} not valid. Uninstall process failed...");
                return ActionResult.Failure;
            }

            session.Log("Uninstall key verified. Proceeding further...");
            return ActionResult.Success;
        }
    }
}
