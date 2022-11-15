using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common.Utils
{
    public static class CommonUtils
    {
        public static string FilePathSlash => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        public static string RootFolder => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static string GetAbsoletePath(string path) => Path.GetFullPath(new Uri(Path.Combine(RootFolder, path)).LocalPath);

        public static string ConfigFolder => Path.Combine(DataFolder, "configs");

        public static string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Infopercept");

        public static void EnsureFolders()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            if (Directory.Exists(ConfigFolder)) 
            { 
                Directory.CreateDirectory(ConfigFolder); 
            }
        }
    }
}
