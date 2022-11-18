using System;
using Zio.FileSystems;

namespace FileWatcherSample
{
    internal class Program
    {
        static void Main()
        {
            var name = "/mnt/c/watch";
            try
            {
                var fs = new FakeFileSystem();

                var mfs = new MountFileSystem(true);

                mfs.Mount(name, fs);

                Console.ReadLine();

                mfs.Unmount(name);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }       
    }
}
