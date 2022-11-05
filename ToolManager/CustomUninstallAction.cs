using Microsoft.Deployment.WindowsInstaller;

namespace ToolManager
{
    public class CustomUninstallAction
    {
        [CustomAction]
        public static ActionResult CustomAction(Session session)
        {
            session.Log("Hello World! from Uninstall");
            return ActionResult.Success;
        }
    }
}
