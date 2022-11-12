using Microsoft.Deployment.WindowsInstaller;
using System.Windows;

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
            //   session["DBYTES_PASS"]=null;
            if (!string.IsNullOrEmpty(session["DBYTES_PASS"] as string))
            {
                //The code
                MessageBox.Show("Uninstall successfully");
                return ActionResult.Success;

            }
            else
            {
                MessageBox.Show("Stop uninstallation");
                return ActionResult.Failure;

            }
        }
    }
}
