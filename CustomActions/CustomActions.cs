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
            return ActionResult.Success;
        }
    }
}
