using System.IO;
using System.IO.Compression;

namespace Common.Helpers
{
    public static class ZipArchiveHelper
    {
        public static void ExtractZipFileWithOverwrite(string zipPath, string extractPath)
        {
            if (!Directory.Exists(extractPath)) Directory.CreateDirectory(extractPath);

            using (var fs = new FileStream(zipPath, FileMode.Open))
            {
                using (var archive = new ZipArchive(fs))
                {
                    foreach (var entry in archive.Entries)
                    {
                        // Full path for the extracted file
                        var destinationPath = Path.Combine(extractPath, entry.FullName);

                        // Ensure the directory for the extracted file exists
                        var directoryPath = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                        // Check if the file exists and delete it if necessary
                        if (File.Exists(destinationPath)) File.Delete(destinationPath);

                        // Extract the file
                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(destinationPath, FileMode.CreateNew))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}