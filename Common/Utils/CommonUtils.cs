using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common.Utils
{
    public static class CommonUtils
    {
        static CommonUtils()
        {
            if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);

            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

            if (!Directory.Exists(ArtifactsFolder)) Directory.CreateDirectory(ArtifactsFolder);

            if (!Directory.Exists(LogsFolder)) Directory.CreateDirectory(LogsFolder);
        }

        public static string FilePathSlash => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        public static string RootFolder => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static string ConfigFolder => Path.Combine(DataFolder, "configs");

        public static string ArtifactsFolder => Path.Combine(DataFolder, "artifacts");

        public static string LogsFolder => Path.Combine(DataFolder, "logs");

        private static string DataFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Infopercept");

        public static string ConstructFromRoot(string path)
        {
            return Path.GetFullPath(new Uri(Path.Combine(RootFolder, path)).LocalPath);
        }

        public static string GetLogFilePath(string fileName)
        {
            return Path.Combine(LogsFolder, fileName);
        }
    }
}