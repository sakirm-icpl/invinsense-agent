using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Management;
using System.Security.Cryptography;

namespace WinUserManagementTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EnsureFakeUser("user", GetRandomAlphanumericString(16));
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

                if(grp != null)
                {
                    grp.Invoke("Add", new object[] { newUser.Path.ToString() });
                }

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
                if (user.Properties[propertyName].Value != null && user.Properties[propertyName].Value.ToString() == searchTerm)
                {
                    return user;
                }
            }

            return null;
        }

        private static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        private static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
            {
                throw new ArgumentException("length must not be negative", "length");
            }

            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
            {
                throw new ArgumentException("length is too big", "length");
            }

            if (characterSet == null)
            {
                throw new ArgumentNullException("characterSet");
            }

            var characterArray = characterSet.Distinct().ToArray();

            if (characterArray.Length == 0)
            {
                throw new ArgumentException("characterSet must not be empty", "characterSet");
            }

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }

            return new string(result);
        }

        private static void GetCurrentLogin()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogonSession");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Win32_LogonSession instance");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Last Logon: {0}", queryObj["StartTime"]);
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine($"An error occurred while querying for WMI data: {e.Message}");
            }
        }
    }
}
