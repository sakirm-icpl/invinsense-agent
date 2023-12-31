using Microsoft.Win32;
using Serilog;

namespace Common.RegistryHelpers
{
    public static class ToolRegistry
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ToolRegistry));

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