using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;

namespace UserSessionAppMonitor
{
    public partial class AppMonitorService : ServiceBase
    {
        private Timer checkTimer;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType, int ImpersonationLevel, ref IntPtr phNewToken);

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSQueryUserToken(uint sessionId, out IntPtr Token);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        private enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public uint cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint WTSGetActiveConsoleSessionId();

        const int SecurityImpersonation = 2;
        const int TokenPrimary = 1;
        const uint GENERIC_ALL_ACCESS = 0x10000000;
        const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        private void CheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var processes = Process.GetProcesses();
            bool isSessionActive = processes.Any(p => p.SessionId > 0 && p.ProcessName != "Idel");
            EventLog.WriteEntry($"Is session active: {isSessionActive}");

            if(!isSessionActive)
            {
                EventLog.WriteEntry("Returning from function...");
                return;
            }

            Process userSessionApp = processes.FirstOrDefault(pp => pp.ProcessName.StartsWith("UserSessionApp"));

            if(userSessionApp != null)
            {
                EventLog.WriteEntry($"UserSessionApp: {userSessionApp.ProcessName} - {userSessionApp.SessionId}");
                return;
            }

            EventLog.WriteEntry("Trying to start UserSessionApp...");

            IntPtr Token = new IntPtr(0);
            IntPtr DupedToken = new IntPtr(0);
            bool ret;
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (uint)Marshal.SizeOf(si);
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.nLength = (uint)Marshal.SizeOf(sa);

            ret = WTSQueryUserToken((uint)WTSGetActiveConsoleSessionId(), out Token);
            if (!ret)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WTSQueryUserToken failed");
            }

            ret = DuplicateTokenEx(Token, GENERIC_ALL_ACCESS, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)TOKEN_TYPE.TokenPrimary, ref DupedToken);

            if (!ret)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "DuplicateTokenEx failed");
            }

            IntPtr lpEnvironment = IntPtr.Zero;
            ret = CreateEnvironmentBlock(ref lpEnvironment, DupedToken, false);
            if (!ret)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateEnvironmentBlock failed");
            }

            string appName = @"C:\path\to\your\app\myApp.exe";

            ret = CreateProcessAsUser(DupedToken, appName, null, ref sa, ref sa, false, CREATE_UNICODE_ENVIRONMENT, lpEnvironment, null, ref si, out pi);
            if (!ret)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateProcessAsUser failed");
            }

            Console.WriteLine("CreateProcessAsUser succeeded: " + pi.dwProcessId);
        }

        public AppMonitorService()
        {
            InitializeComponent();
            checkTimer = new Timer(10000); // check every 10 seconds
            checkTimer.Elapsed += new ElapsedEventHandler(CheckTimerElapsed);

        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service started");
            checkTimer.Start();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Service stopped");
            checkTimer.Stop();
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
    }
}
