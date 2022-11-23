using System;
using System.DirectoryServices;

namespace WinUserManagementTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EnsureFakeUser("user", "P@$$Word");
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

                DirectoryEntry grp = AD.Children.Find("Administrators", "group");
                grp?.Invoke("Add", new object[] { newUser.Path.ToString() });

                Console.WriteLine("Account Created Successfully");
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
