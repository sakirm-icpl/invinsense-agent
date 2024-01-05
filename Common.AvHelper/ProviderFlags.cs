using System;

namespace Common.AvHelper
{
    [Flags]
    public enum ProviderFlags : byte
    {
        FIREWALL = 1,
        AUTOUPDATE_SETTINGS = 2,
        ANTIVIRUS = 4,
        ANTISPYWARE = 8,
        INTERNET_SETTINGS = 16,
        USER_ACCOUNT_CONTROL = 32,
        SERVICE = 64,
        NONE = 0,
    }
}
