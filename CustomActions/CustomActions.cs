using Microsoft.Deployment.WindowsInstaller;

namespace ToolManager
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckSessionParameters(Session session)
        {
            session.Log("Hello World! from Install");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveTools(Session session)
        {
            session.Log("Hello World! from Uninstall");
            if (session["UNINSTALL_KEY"] == "ICPL_2023")
            {
                session.Log("Uninstall Successfully");
                return ActionResult.Success;
            }
            else if (!string.IsNullOrEmpty(session["UNINSTALL_KEY"] as string))
            {
                session.Log("Uninstall failed");
                return ActionResult.Failure;
            }
            else
            {
                session.Log("Uninstall failed");
                return ActionResult.Failure;

            }
        }
    }
}
