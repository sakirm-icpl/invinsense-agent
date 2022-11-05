using Microsoft.Deployment.WindowsInstaller;

namespace ToolManager
{
    public class CustomInstallAction
    {
        [CustomAction]
        public static ActionResult CustomAction(Session session)
        {
            session.Log("Hello World! from Install");
            return ActionResult.Success;
        }
    }
}
