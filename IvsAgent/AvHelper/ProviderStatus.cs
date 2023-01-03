using System.Runtime.InteropServices;

namespace IvsAgent.AvHelper
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
