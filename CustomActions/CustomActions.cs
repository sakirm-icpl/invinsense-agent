using Microsoft.Deployment.WindowsInstaller;

namespace ToolManager
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckSessionParameters(Session session)
        {
            session.Log("Preparing Install");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveTools(Session session)
        {
            session.Log("Preparing Uninstall");

            if (session["UNINSTALL_KEY"] == "ICPL_2023")
            {
                session.Log("Uninstall key verified. Proceeding further...");
                return ActionResult.Success;
            }
            else
            {
                session.Log("Uninstall key not supplied. Uninstall process failed...");
                return ActionResult.Failure;

            }
        }
    }
}
