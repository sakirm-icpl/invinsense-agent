using System;

namespace AvMonitorTest
{
    [Flags]
    public enum AVStatusFlags : byte
    {
        Unknown = 1,
        Enabled = 16,
        Disabled = 32
    }
}