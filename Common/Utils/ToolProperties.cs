using Microsoft.Win32;

namespace Common.Utils
{
    public class ToolProperties
    {
        public static string GetPropertyByName(string name)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Infopercept", false)) // False is important!
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
    }
}
