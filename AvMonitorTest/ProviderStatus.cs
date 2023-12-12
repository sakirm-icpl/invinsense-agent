using System.Runtime.InteropServices;

namespace AvMonitorTest
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProviderStatus
    {
        public SignatureStatusFlags SignatureStatus;
        public AVStatusFlags AVStatus;
        public ProviderFlags SecurityProvider;
        public byte unused;
    }
}