using System;
using System.Runtime.InteropServices;

namespace ToolManager
{
    public static class UserSessionHelper
    {
        public static bool IsUserSessionActive()
        {
            int sessionId = WTSGetActiveConsoleSessionId();
            if (sessionId == -1) return false; // No active session

            WTS_CONNECTSTATE_CLASS sessionState;
            IntPtr buffer;
            int bytesReturned;
            bool result = WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSConnectState, out buffer, out bytesReturned);

            if (!result) return false;

            sessionState = (WTS_CONNECTSTATE_CLASS)Marshal.ReadInt32(buffer);
            WTSFreeMemory(buffer);

            return sessionState == WTS_CONNECTSTATE_CLASS.WTSActive;
        }

        // P/Invoke declarations
        [DllImport("wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("kernel32.dll")]
        private static extern int WTSGetActiveConsoleSessionId();

        private enum WTS_CONNECTSTATE_CLASS
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

        private enum WTS_INFO_CLASS
        {
            WTSConnectState
        }
    }
}
