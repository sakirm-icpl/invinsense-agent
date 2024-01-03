using Microsoft.Win32;
using Serilog;

namespace Common.RegistryHelpers
{
    public static class WinRegistryHelper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(WinRegistryHelper));

        /// <summary>
        /// reg.local.PATH.KEY
        ///     reg = RegistryView.Registry32
        ///     reg64 = RegistryView.Registry64
        ///     Local = RegistryHive.LocalMachine
        ///     PATH = Open subkey
        ///     Key = Key value
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPropertyByTemplate(string path)
        {
            var parts = path.Split('.');
            try
            {
                var regView = parts[0] == "reg64" ? RegistryView.Registry64 : RegistryView.Registry32;
                var regHive = parts[1] == "Local" ? RegistryHive.LocalMachine : RegistryHive.CurrentUser;
                var keyPath = parts[2];
                var key = parts[3];

                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var subkey = baseKey.OpenSubKey($"SOFTWARE\\{keyPath}", false)) // False is important!
                {
                    var value = subkey?.GetValue(key) as string;
                    return value;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetPropertyByName(string path, string name)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var subKey = baseKey.OpenSubKey($"SOFTWARE\\{path}", false)) // False is important!
                {
                    var value = subKey?.GetValue(name) as string;
                    return value;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void SetPropertyByName(string path, string name, string value)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var subKey = baseKey.OpenSubKey($"SOFTWARE\\{path}", true))
                {
                    if (subKey == null)
                    {
                        var newKey = baseKey.CreateSubKey($"SOFTWARE\\{path}");
                        newKey.SetValue(name, value);
                    }
                    else
                    {
                        subKey.SetValue(name, value);
                    }
                }
            }
            catch
            {
                _logger.Error($"Error in set registry value:{path} {name} {value}");
            }
        }
    }
}