using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinWinFileModule: Creates, touches or removes files or directories
    ///   ansible.windows.win_file:
    ///     path: File or directory path to create, delete or modify permissions on
    ///     state: absent or present (default)
    /// </summary>
    internal class WinFileModule : ModuleBase
    {
        private WinFileModule() : base(WinModuleNames.WIN_FILE)
        {
        }

        public static WinFileModule Create(Dictionary<object, object> dict)
        {
            var module = new WinFileModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("path"))
            {
                module.Path = dict["path"].ToString();
            }

            if (dict.ContainsKey("state"))
            {
                module.State = dict["state"].ToString();
            }

            if (dict.ContainsKey("recurse"))
            {
                module.Recurse = dict["recurse"].ToString();
            }

            if (dict.ContainsKey("force"))
            {
                module.Force = dict["force"].ToString();
            }

            if (dict.ContainsKey("src"))
            {
                module.Src = dict["src"].ToString();
            }

            if (dict.ContainsKey("dest"))
            {
                module.Dest = dict["dest"].ToString();
            }

            return module;
        }

        public override bool Validate()
        {
            return true;
        }

        public override void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("WinWinFileModule: Creates, touches or removes files or directories");
            Console.ResetColor();
        }

        public string Path { get; set; }
        public string State { get; set; }
        public string Recurse { get; set; }
        public string Force { get; set; }
        public string Src { get; set; }
        public string Dest { get; set; }

        public override string ToString()
        {
            return $"{ModuleName}:\n" +
                   $"    path: {Path}\n" +
                   $"    state: {State}\n" +
                   $"    recurse: {Recurse}\n" +
                   $"    force: {Force}\n" +
                   $"    src: {Src}\n" +
                   $"    dest: {Dest}";

        }
    }
}
