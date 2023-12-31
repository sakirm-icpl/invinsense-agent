using System;
using System.Text.RegularExpressions;

namespace Common.Encryption
{
    internal class Common
    {
        public static bool GetVersionFromName(string filePath, out Version version)
        {
            // Use a regular expression to find version numbers in the file name
            var match = Regex.Match(filePath, @"(\d+\.\d+\.\d+)");
            if (match.Success)
            {
                version = new Version(match.Groups[1].Value);
                //Log.Information("Found MSI version {0} in file name {1}", version, filePath);
                return true;
            }

            version = null;
            return false;
        }
    }
}