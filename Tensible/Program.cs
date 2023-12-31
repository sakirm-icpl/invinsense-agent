using System;
using System.Linq;
using Tensible.Models;

namespace Tensible
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("-v") || args.Contains("--version"))
            {
                Console.WriteLine("TensibleWrapper v0.1");
                return;
            }

            var varIndex = Array.IndexOf(args, "--extra-vars") + 1;

            if (varIndex != 0)
            {
                var varFile = args[varIndex].Trim('"');

                if (!varFile.EndsWith(".yml") || !varFile.StartsWith("@"))
                {
                    Console.WriteLine("Extra vars file must be a .yml file");
                    return;
                }

                varFile = varFile.TrimStart('@');

                var content = YmlHelper.ReadVariableFile(varFile);

                foreach (var entry in content)
                {
                    Console.WriteLine($"{entry.Key}: {entry.Value}");
                }
            }

            var pbFile = args[args.Length - 1].Trim('"');

            var playbooks = YmlHelper.ReadPlaybook(pbFile);

            Console.WriteLine($"Playbook found: {playbooks.Count()}");

            foreach (var pb in playbooks)
            {
                foreach (var task in pb.Tasks)
                {
                    var winTask = new TensibleTask(task);
                    ColoredConsole.WriteLine(winTask.ToString(), ConsoleColor.Gray);
                }
            }

            foreach (var pb in playbooks)
            {
                foreach (var task in pb.Tasks)
                {
                    var winTask = new TensibleTask(task);

                    //Running module
                    winTask.Module.Execute();
                }
            }

            Console.WriteLine("Done");
        }
    }
}
