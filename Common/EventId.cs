using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public enum EventId
    {
        InvinsenseServiceNotDetected = 10,
        InvinsenseServiceStopped = 11,
        InvinsenseServiceError = 12,
        InvinsenseServiceWarning = 13,
        InvinsenseServiceRunning = 14,

        WazuhServiceNotDetected,
        WazuhServiceStopped,
        WazuhServiceError,
        WazuhServiceWarning,
        WazuhServiceRunning
    }
}
