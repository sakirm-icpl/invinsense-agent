using System.IO.Compression;
using System.IO;

namespace Common.Helpers
{
    public static class ZipArchiveHelper
    {
        public static void ExtractZipFileWithOverwrite(string zipPath, string extractPath)
        {
            if(!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            using (FileStream fs = new FileStream(zipPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(fs))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Full path for the extracted file
                        string destinationPath = Path.Combine(extractPath, entry.FullName);

                        // Ensure the directory for the extracted file exists
                        string directoryPath = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Check if the file exists and delete it if necessary
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }

                        // Extract the file
                        using (Stream entryStream = entry.Open())
                        using (FileStream fileStream = new FileStream(destinationPath, FileMode.CreateNew))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}
