using System;

namespace IvsAgent.AvHelper
{
    [Flags]
    public enum SignatureStatusFlags : byte
    {
        UpToDate = 0,
        OutOfDate = 16
    }
}
