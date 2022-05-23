using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SingleAgent.Utils
{
    public static class CommonUtils
    {
        public static string FilePathSlash => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        public static string RootFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + FilePathSlash;

        public static string ConfigFolder => RootFolder + "configs" + FilePathSlash;

        public static string ReportFolder => RootFolder + "reports" + FilePathSlash;

        public static string QueryFolder => RootFolder + "queries" + FilePathSlash;

    }
}
