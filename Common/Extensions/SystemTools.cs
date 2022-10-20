using Serilog;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;

namespace Common.Extensions
{
    internal class SystemTools
    {
        private static readonly ILogger _logger = Log.ForContext<SystemTools>();

        public static void CreateFakeUser(string Name)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                DirectoryEntry NewUser = AD.Children.Add(Name, "user");
                NewUser.Invoke("Put", new object[] { "Description", "Test User from IvsTray" });
                NewUser.CommitChanges();
                DirectoryEntry grp;

                grp = AD.Children.Find("Administrators", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }
        }

        public static void RemoveFakeUser(string Name)
        {
            DirectoryEntry localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName.ToString());
            DirectoryEntries users = localDirectory.Children;
            DirectoryEntry user = users.Find(Name);
            users.Remove(user);
        }

        public void CreateFakeFiles()
        {
            var dir = @"C:\Single Agent\WindowUser";  // folder location
            if (!Directory.Exists(dir))  // if it doesn't exist, create
            {
                Directory.CreateDirectory(dir);
            }   

            // use Path.Combine to combine 2 strings to a path
            File.WriteAllText(Path.Combine(dir, "WindowUser.txt"), "Username-Single Agent");
        }

        public static void RemoveFakeFiles()
        {
            var dir = @"C:\Single Agent";
            Directory.Delete(dir, true);
        }

        public static void KillProcess()
        {
            foreach (var process in Process.GetProcessesByName("IvsTray"))
            {
                process.Kill();
            }
        }
    }
}
