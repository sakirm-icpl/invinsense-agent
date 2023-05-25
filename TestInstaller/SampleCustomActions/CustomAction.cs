using WixToolset.Dtf.WindowsInstaller;

namespace SampleCustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckSessionParameters(Session session)
        {
            session.Log("Begin CheckSessionParameters");

            if (string.IsNullOrEmpty(session["REQUIRED"]))
            {
                session.Log("Parameter REQUIRED value not found.");
                session.Log("Application install should not proceed.");
                return ActionResult.Failure;
            }

            session.Log("Parameter REQUIRED value: " + session["REQUIRED"]);

            session.Log("End CheckSessionParameters");

            return ActionResult.Success;
        }
    }
}
