using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Common.FileHelpers
{
    public static class CommonFileHelpers
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext(typeof(CommonFileHelpers));

        public static bool GetFileVersion(string path, out Version version)
        {
            if (!File.Exists(path))
            {
                version = null;
                return false;
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(path);

            var regex = new Regex(@"[vV]?(?<version>\d+(\.\d+)+)");
            Match match = regex.Match(versionInfo.ProductVersion);
            if (match.Success)
            {
                version = new Version(match.Groups["version"].Value);
                return true;
            }
         
            _logger.Error("Failed to parse version from {0}", versionInfo.ProductVersion);
            version = null;
            return false;
        }

        public static DateTime? GetFileDate(string path)
        {
            if (!File.Exists(path)) return null;
            return File.GetCreationTime(path);
        }

        public static DateTime? GetDirectoryDate(string path)
        {
            if (!Directory.Exists(path)) return null;
            return Directory.GetCreationTime(path);
        }

        public static void EnsureSourceToDestination(string sourcePath, string destinationPath)
        {
            _logger.Information($"Copying {sourcePath} to {destinationPath}");
            if (!File.Exists(sourcePath))
            {
                _logger.Error($"{sourcePath} not found");
                return;
            }

            File.Copy(sourcePath, destinationPath, true);
        }

        public static void ExtractSourceToDestination(string sourcePath, string destinationPath)
        {
            _logger.Information($"Extracting {sourcePath} to {destinationPath}");
            if (!File.Exists(sourcePath))
            {
                _logger.Error($"{sourcePath} not found");
                return;
            }

            ZipArchiveHelper.ExtractZipFileWithOverwrite(sourcePath, destinationPath);
        }

        public static string GetLatestPath(string folderPath, string filePrefix, string extension)
        {
            var files = Directory.GetFiles(folderPath, $"{filePrefix}-*.{extension}");

            _logger.Information($"Found files: {string.Join(",", files)}");

            if (files.Length == 0)
            {
                _logger.Error("No msi file found in artifacts folder");
                return string.Empty;
            }

            if (files.Length > 1)
            {
                _logger.Error("More than one msi file found in artifacts folder");

                // Sort the files by version number in descending order
                Array.Sort(files, (a, b) =>
                {
                    // Extract version numbers from file names using a regular expression
                    var regex = new Regex($@"{filePrefix}-(\d+\.\d+\.\d+)\.{extension}");
                    var matchA = regex.Match(Path.GetFileName(a));
                    var matchB = regex.Match(Path.GetFileName(b));

                    if (matchA.Success && matchB.Success)
                    {
                        var versionA = new Version(matchA.Groups[1].Value);
                        var versionB = new Version(matchB.Groups[1].Value);

                        return versionB.CompareTo(versionA); // Sort in descending order
                    }

                    return 0; // Default to no change in order
                });

                // Keep the latest file and delete the rest
                for (var i = 1; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                    _logger.Information($"Deleted: {files[i]}");
                }

                _logger.Information("Old files deleted.");
            }
            else
            {
                _logger.Information("No old files to delete.");
            }

            _logger.Information($"Using file: {files[0]}");

            return files[0];
        }
    }
}