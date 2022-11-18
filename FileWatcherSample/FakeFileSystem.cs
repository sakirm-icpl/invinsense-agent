using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zio;

namespace FileWatcherSample
{
    public class FakeFileSystem : IFileSystem
    {
        public bool CanWatch(UPath path)
        {
            return true;
        }

        public UPath ConvertPathFromInternal(string systemPath)
        {
            return "c:\\watch";
        }

        public string ConvertPathToInternal(UPath path)
        {
            return path.ToString();
        }

        public void CopyFile(UPath srcPath, UPath destPath, bool overwrite)
        {
            return;
        }

        public void CreateDirectory(UPath path)
        {
            return;
        }

        public void DeleteDirectory(UPath path, bool isRecursive)
        {
            return;
        }

        public void DeleteFile(UPath path)
        {
            return;
        }

        public bool DirectoryExists(UPath path)
        {
            return true;
        }

        public void Dispose()
        {
            return;
        }

        public IEnumerable<FileSystemItem> EnumerateItems(UPath path, SearchOption searchOption, SearchPredicate searchPredicate = null)
        {
            var items = new List<FileSystemItem>
            {
                new FileSystemItem(this, "users.txt", false),
                new FileSystemItem(this, "secret", true)
            };

            return items;
        }

        public IEnumerable<UPath> EnumeratePaths(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
        {
            var paths = new List<UPath>
            {
                "users.txt",
                "secret"
            };

            return paths;
        }

        public bool FileExists(UPath path)
        {
            return true;
        }

        public FileAttributes GetAttributes(UPath path)
        {
            return FileAttributes.Normal;
        }

        public DateTime GetCreationTime(UPath path)
        {
            return DateTime.Now;
        }

        public long GetFileLength(UPath path)
        {
            return 100;
        }

        public DateTime GetLastAccessTime(UPath path)
        {
            return DateTime.Now;
        }

        public DateTime GetLastWriteTime(UPath path)
        {
            return DateTime.Now;
        }

        public void MoveDirectory(UPath srcPath, UPath destPath)
        {
            return;
        }

        public void MoveFile(UPath srcPath, UPath destPath)
        {
            return;
        }

        public Stream OpenFile(UPath path, FileMode mode, FileAccess access, FileShare share = FileShare.None)
        {
            string test = "Testing 1-2-3";

            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        public void ReplaceFile(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
        {
            return;
        }

        public void SetAttributes(UPath path, FileAttributes attributes)
        {
            return;
        }

        public void SetCreationTime(UPath path, DateTime time)
        {
            return;
        }

        public void SetLastAccessTime(UPath path, DateTime time)
        {
            return;
        }

        public void SetLastWriteTime(UPath path, DateTime time)
        {
            return;
        }

        public IFileSystemWatcher Watch(UPath path)
        {
            return null;
        }
    }
}
