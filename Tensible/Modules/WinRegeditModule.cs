using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinRegeditModule: Manage and query Windows registry
    ///   ansible.windows.win_regedit:
    ///     path: Name of the registry path.
    ///     name: Name of the registry entry in the above path parameters.
    ///     data: Value of the registry entry name in path.
    ///     type: The registry value data type.
    ///     state: The state of the registry entry.
    ///     hive: A path to a hive key like C:\Users\Default\NTUSER.DAT to load in the registry.
    /// </summary>
    internal class WinRegeditModule : ModuleBase
    {
        private WinRegeditModule() : base(WinModuleNames.WIN_REGEDIT)
        {
            Type = "string";
            State = "present";
        }

        public static WinRegeditModule Create(Dictionary<object, object> dict)
        {
            var module = new WinRegeditModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("path"))
            {
                module.Path = dict["path"].ToString();
            }

            if (dict.ContainsKey("name"))
            {
                module.Name = dict["name"].ToString();
            }

            if (dict.ContainsKey("data"))
            {
                module.Data = dict["data"].ToString();
            }

            if (dict.ContainsKey("type"))
            {
                module.Type = dict["type"].ToString();
            }

            if (dict.ContainsKey("state"))
            {
                module.State = dict["state"].ToString();
            }

            if (dict.ContainsKey("hive"))
            {
                module.Hive = dict["hive"].ToString();
            }

            return module;
        }

        public string Path { get; set; }
       
        public string Name { get; set; }
        
        public string Data { get; set; }
        
        public string Type { get; set; }
        
        public string State { get; set; }
        
        public string Hive { get; set; }

        public override bool Validate()
        {
            return true;
        }

        public override void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Executing {ModuleName} module...");
            Console.ResetColor();
        }

        public override string ToString()
        {
            return $"{ModuleName}:\n" +
                   $"    path: {Path}\n" +
                   $"    name: {Name}\n" +
                   $"    data: {Data}\n" +
                   $"    type: {Type}\n" +
                   $"    state: {State}\n" +
                   $"    hive: {Hive}";
        }
    }
}
