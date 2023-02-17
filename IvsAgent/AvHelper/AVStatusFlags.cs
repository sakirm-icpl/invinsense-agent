using System;

namespace IvsAgent.AvHelper
{
    [Flags]
    public enum AVStatusFlags : byte
    {
        Unknown = 1,
        Enabled = 16,
        Disable = 32,
    }
}
