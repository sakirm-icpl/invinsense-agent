using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinUserLogOffTest
{
    /// <summary>
    ///     https://stackoverflow.com/questions/5207506/logoff-interactive-users-in-windows-from-a-service
    /// </summary>
    internal class Program
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] string pServerName);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern int WTSEnumerateSessions(IntPtr hServer, [MarshalAs(UnmanagedType.U4)] int Reserved,
            [MarshalAs(UnmanagedType.U4)] int Version, ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref int pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        internal static List<int> GetSessionIDs(IntPtr server)
        {
            var sessionIds = new List<int>();
            var buffer = IntPtr.Zero;
            var count = 0;
            var retval = WTSEnumerateSessions(server, 0, 1, ref buffer, ref count);
            var dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            long current = (int)buffer;

            if (retval != 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;
                    sessionIds.Add(si.SessionID);
                }

                WTSFreeMemory(buffer);
            }

            return sessionIds;
        }

        internal static bool LogOffUser(string userName, IntPtr server)
        {
            userName = userName.Trim().ToUpper();
            var sessions = GetSessionIDs(server);
            var userSessionDictionary = GetUserSessionDictionary(server, sessions);
            if (userSessionDictionary.ContainsKey(userName))
                return WTSLogoffSession(server, userSessionDictionary[userName], true);
            return false;
        }

        private static Dictionary<string, int> GetUserSessionDictionary(IntPtr server, List<int> sessions)
        {
            var userSession = new Dictionary<string, int>();

            foreach (var sessionId in sessions)
            {
                var uName = GetUserName(sessionId, server);
                if (!string.IsNullOrWhiteSpace(uName))
                    userSession.Add(uName, sessionId);
            }

            return userSession;
        }

        internal static string GetUserName(int sessionId, IntPtr server)
        {
            var buffer = IntPtr.Zero;
            uint count = 0;
            var userName = string.Empty;
            try
            {
                WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out count);
                userName = Marshal.PtrToStringAnsi(buffer).ToUpper().Trim();
            }
            finally
            {
                WTSFreeMemory(buffer);
            }

            return userName;
        }

        private static void Main()
        {
            Console.Write("Enter ServerName<Enter 0 to default to local>:");
            var input = Console.ReadLine().Trim();

            var server = WTSOpenServer(input == "0" || input == "" ? Environment.MachineName : input);

            try
            {
                do
                {
                    Console.WriteLine("Please Enter L => list sessions, G => Logoff a user, Q => exit.");

                    input = Console.ReadLine().ToUpper();

                    if (string.IsNullOrWhiteSpace(input)) continue;

                    if (input == "L")
                    {
                        var userSessionDict = GetUserSessionDictionary(server, GetSessionIDs(server));
                        foreach (var userSession in userSessionDict)
                            Console.WriteLine("{0} is logged in {1} session", userSession.Key, userSession.Value);
                    }
                    else if (input == "G")
                    {
                        Console.Write("Enter UserName:");
                        input = Console.ReadLine();
                        LogOffUser(input, server);
                    }
                } while (input.ToUpper() != "Q");
            }
            finally
            {
                WTSCloseServer(server);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)] public String pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        internal enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        internal enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo
        }
    }
}