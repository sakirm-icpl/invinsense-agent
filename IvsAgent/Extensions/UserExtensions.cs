using Serilog;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;

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

        /// <summary>
        /// TODO: Check windows behavior without user password and ensure we can restrict user to login and isolate system.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void EnsureFakeUser(string username, string password)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");

                DirectoryEntry directoryUser = Search(username, "Name") ?? AD.Children.Add(username, "user");
                directoryUser.Invoke("SetPassword", new object[] { GetRandomAlphanumericString(12) });
                directoryUser.Invoke("Put", new object[] { "Description", "Maintenance User" });
                directoryUser.CommitChanges();

                try
                {
                    DirectoryEntry grp = AD.Children.Find("Users", "group");
                    grp?.Invoke("Add", new object[] { directoryUser.Path.ToString() });
                } catch{ }

                _logger.Information("Account Created Successfully");

                _logger.Information("Adding fake file");
                FileFaker.EnsureUserCredentialInFile(username, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
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
