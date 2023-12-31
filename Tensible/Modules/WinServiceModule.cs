using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinServiceModule: Manage and query Windows services
    ///   ansible.windows.win_package:
    ///     path: Url or local file path of msi file
    ///     product_id: Product Id
    ///     state: present / absent
    ///     log_path: Optional path to log file
    /// </summary>
    internal class WinServiceModule : ModuleBase
    {
        private WinServiceModule() : base(WinModuleNames.WIN_SERVICE)
        {
        }

        public static WinServiceModule Create(Dictionary<object, object> dict)
        {
            var module = new WinServiceModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("path"))
            {
                module.Path = dict["path"].ToString();
            }

            if (dict.ContainsKey("product_id"))
            {
                module.ProductId = dict["product_id"].ToString();
            }

            if (dict.ContainsKey("state"))
            {
                module.State = dict["state"].ToString();
            }

            if (dict.ContainsKey("log_path"))
            {
                module.LogPath = dict["log_path"].ToString();
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
            Console.WriteLine($"Executing {nameof(WinServiceModule)}...");
            Console.ResetColor();
        }

        public string Path { get; set; }

        public string ProductId { get; set; }

        public string State { get; set; }

        public string LogPath { get; set; }

        public override string ToString()
        {
            return $"{ModuleName}:\n" +
                   $"    path: {Path}\n" +
                   $"    product_id: {ProductId}\n" +
                   $"    state: {State}\n" +
                   $"    log_path: {LogPath}";
        }
    }
}
