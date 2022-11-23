using System.IO;

namespace IvsAgent.Extensions
{
    internal class FileFaker
    {
        public static void CreateFile()
        {
            var dir = @"C:\Users";
            File.WriteAllText(Path.Combine(dir, "Users.txt"), "maintenance  P@$$w0rd");
        }
    }
}
