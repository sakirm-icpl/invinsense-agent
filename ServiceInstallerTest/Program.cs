using System;
using System.IO;
using System.Reflection;

namespace ServiceInstallerTest
{
    internal class Program
    {
        private static readonly string path = Path.GetFullPath(new Uri(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
            "..\\..\\..\\SampleService\\bin\\Debug\\SampleService.exe")).LocalPath);

        private static readonly string _serviceName = "SimpleService";

        private static void Main(string[] args)
        {
            Console.WriteLine("Please type command");

            string input;

            while ((input = Console.ReadLine().ToLower()) != "q")
                switch (input)
                {
                    case "i":
                        ScWrapper.Install(path, _serviceName);
                        break;
                    case "u":
                        ScWrapper.Uninstall(path, _serviceName);
                        break;
                    case "r":
                        ScWrapper.Start(_serviceName);
                        break;
                    case "s":
                        ScWrapper.Stop(_serviceName);
                        break;
                    case "p":
                        ScWrapper.Pause(_serviceName);
                        break;
                    case "c":
                        ScWrapper.Resume(_serviceName);
                        break;
                    case "x":
                        ScWrapper.SendCommand(_serviceName, 130);
                        break;
                    case "x 1":
                        ScWrapper.SendCommand(_serviceName, 131);
                        break;
                    case "x 2":
                        ScWrapper.SendCommand(_serviceName, 132);
                        break;
                    default:
                        Console.WriteLine("Please select correct switch");
                        break;
                }
        }
    }
}