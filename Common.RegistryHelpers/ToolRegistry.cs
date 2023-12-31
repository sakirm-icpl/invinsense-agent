using Microsoft.Win32;
using Serilog;
using System;

namespace Common.RegistryHelpers
{
    public static class ToolRegistry
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ToolRegistry));

        public static bool CanSkipMonitoring(string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{name}", false)) // False is important!
                {
                    var skipMonitoring = key?.GetValue("SKIP_MONITORING") as string ?? "N";
                    return skipMonitoring == "Y" || skipMonitoring == "y";
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in reading tool status. {ex}");
            }

            return false;
        }

        public static string GetPropertyByName(string path, string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{path}", false)) // False is important!
                {
                    var value = key?.GetValue(name) as string;
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
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey($"SOFTWARE\\Infopercept\\{path}", true))
                {
                    if (key == null)
                    {
                        var newKey = hklm.CreateSubKey($"SOFTWARE\\Infopercept\\{path}");
                        newKey.SetValue(name, value);
                    }
                    else
                    {
                        key.SetValue(name, value);
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