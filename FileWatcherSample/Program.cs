using System;
using Zio.FileSystems;


namespace FileWatcherSample
{
    /// <summary>
    /// https://github.com/tusharjain1082/FilediskProxy.Net
    /// https://learn.microsoft.com/en-us/uwp/api/windows.storage.provider.storageprovidersyncrootmanager?view=winrt-22621
    /// 
    /// </summary>
    internal class Program
    {
        static void Main()
        {
            var name = "/mnt/c/watch";
            try
            {
                /*
                var fs = new FakeFileSystem();

                var mfs = new MountFileSystem(true);

                mfs.Mount(name, fs);

                Console.ReadLine();

                mfs.Unmount(name);
                */

                var fs = new MemoryFileSystem();
                
                var mfs = new MountFileSystem(true);

                mfs.Mount(name, fs);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }       
    }
}
