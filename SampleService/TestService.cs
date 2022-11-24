using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace SampleService
{
    public partial class TestService : ServiceBase
    {
        public TestService()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
            CanStop = false;
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service started");
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Service stopped");
            base.OnStop();
        }

        protected override void OnPause()
        {
            EventLog.WriteEntry("Service Pause");
            base.OnPause();
        }

        protected override void OnContinue()
        {
            EventLog.WriteEntry("Service On continue");
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            EventLog.WriteEntry("Service On Shutdown");
            base.OnShutdown();
        }

        
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            EventLog.WriteEntry("Service stopped");
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnCustomCommand(int command)
        {
            EventLog.WriteEntry($"Service command: {command}");

            /*
            if(command == 130)
            {
                Stop();
            }
            */

            if(command == 131)
            {
                CanStop = true;
            }
            else if (command == 132)
            {
                CanStop = false;
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry($"Service:OnSessionChange {DateTime.Now.ToLongTimeString()} - Session change notice received: {changeDescription.Reason}  Session ID: {changeDescription.SessionId}");

            string username = GetUsername(changeDescription.SessionId);

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    EventLog.WriteEntry($"Service.OnSessionChange: Logon - {username}");
                    break;

                case SessionChangeReason.SessionLogoff:
                    EventLog.WriteEntry($"Service.OnSessionChange Logoff - {username}");
                    break;
            }
        }

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        
        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);

        private enum WtsInfoClass
        {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        /// <summary>
        /// https://smbadiwe.github.io/post/track-activities-windows-service/
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="prependDomain"></param>
        /// <returns></returns>
        private static string GetUsername(int sessionId, bool prependDomain = true)
        {
            string username = "SYSTEM";
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out IntPtr buffer, out int strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (prependDomain)
                {
                    if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1)
                    {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }
    }
}
