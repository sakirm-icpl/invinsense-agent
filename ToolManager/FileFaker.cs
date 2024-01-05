using System.IO;

namespace ToolManager
{
    internal class FileFaker
    {
        public static void EnsureUserCredentialInFile(string username, string password)
        {
            var dir = @"C:\Users";
            File.WriteAllText(Path.Combine(dir, "Users.txt"), $"{username} {password}");
        }
    }
}
