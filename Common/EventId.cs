namespace Common
{
    public enum EventId
    {
        None = 0,

        IvsNotFound = 10,
        IvsStopped = 11,
        IvsError = 12,
        IvsWarning = 13,
        IvsRunning = 14,

        AvNotFound = 20,
        AvDisabled = 21,
        AvEnabledOutDated = 22,
        AvEnabledUpToDate = 23,

        WazuhNotFound = 30,
        WazuhStopped = 31,
        WazuhError = 32,
        WazuhWarning = 33,
        WazuhRunning = 34,

        DbytesNotFound = 40,
        DbytesStopped = 41,
        DbytesError = 42,
        DbytesWarning = 43,
        DbytesRunning = 44,

        SysmonNotFound = 50,
        SysmonStopped = 51,
        SysmonError = 52,
        SysmonWarning = 53,
        SysmonRunning = 54,

        LmpNotFound = 60,
        LmpStopped = 61,
        LmpError = 62,
        LmpWarning = 63,
        LmpRunning = 64
    }
}
