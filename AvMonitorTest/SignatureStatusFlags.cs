using System;

namespace AvMonitorTest
{
    [Flags]
    public enum SignatureStatusFlags : byte
    {
        UpToDate = 0,
        OutOfDate = 16
    }
}