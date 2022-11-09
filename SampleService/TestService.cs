using System;
using System.ServiceProcess;

namespace SampleService
{
    public partial class TestService : ServiceBase
    {
        public TestService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service started");
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Service stopped");
        }

        protected override void OnCustomCommand(int command)
        {
            EventLog.WriteEntry($"Service command: {command}");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry($"Service:OnSessionChange {DateTime.Now.ToLongTimeString()} - Session change notice received: {changeDescription.Reason}  Session ID: {changeDescription.SessionId}");

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    EventLog.WriteEntry("Service.OnSessionChange: Logon");
                    break;

                case SessionChangeReason.SessionLogoff:
                    EventLog.WriteEntry("Service.OnSessionChange Logoff");
                    break;
            }
        }
    }
}
