using Serilog;
using System;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Management;

namespace IvsAgent.Extensions
{
    internal static class UserExtensions
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(UserExtensions));

        public static string GetCurrentUserName()
        {
            SelectQuery query = new SelectQuery(@"Select * from Win32_Process");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Process in searcher.Get().Cast<ManagementObject>())
                {
                    if (Process["ExecutablePath"] != null && string.Equals(Path.GetFileName(Process["ExecutablePath"].ToString()), "explorer.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] OwnerInfo = new string[2];
                        Process.InvokeMethod("GetOwner", OwnerInfo);

                        return OwnerInfo[0];
                    }
                }
            }
            return "";
        }

        public static void EnsureFakeUser(string username, string password)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");

                DirectoryEntry newUser = Search(username, "Name");

                if (newUser == null)
                {
                    newUser = AD.Children.Add(username, "user");
                }

                newUser.Invoke("SetPassword", new object[] { password });
                newUser.Invoke("Put", new object[] { "Description", "Maintenance User" });
                newUser.CommitChanges();

                try
                {
                    DirectoryEntry grp = AD.Children.Find("Users", "group");
                    grp?.Invoke("Add", new object[] { newUser.Path.ToString() });
                } catch{ }

                _logger.Information("Account Created Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();

            }

        }

        private static DirectoryEntry Search(string searchTerm, string propertyName)
        {
            DirectoryEntry directoryObject = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");

            foreach (DirectoryEntry user in directoryObject.Children)
            {
                if (user.Properties[propertyName].Value != null)
                    if (user.Properties[propertyName].Value.ToString() == searchTerm)
                        return user;
            }

            return null;
        }
    }
}
