using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinPackageModule: Manage and query Windows packages
    ///   ansible.windows.win_package:
    ///     provider: chocolatey / msi / appx / msu / winget
    ///     path: Url or local file path of msi file
    ///     product_id: Product Id of the package
    ///     arguments: Command line arguments passed to the installer
    ///     state: present / absent 
    ///     log_path: Optional path to log file
    ///     validate_certs: Whether to validate server certificate, default is true
    ///     creates_service: Will check the existing of the service specified and use the result to determine whether the package is already installed.
    ///     expected_return_code: [0, 666, 3010]
    /// </summary>
    internal class WinPackageModule : ModuleBase
    {
        private WinPackageModule() : base(WinModuleNames.WIN_PACKAGE)
        {
            Provider = "msi";
            ValidateCerts = true;
        }

        public static WinPackageModule Create(Dictionary<object, object> dict)
        {
            var module = new WinPackageModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("provider"))
            {
                module.Provider = dict["provider"].ToString();
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

            if (dict.ContainsKey("creates_service"))
            {
                module.CreatesService = dict["creates_service"].ToString();
            }

            if (dict.ContainsKey("validate_certs"))
            {
                module.ValidateCerts = Convert.ToBoolean(dict["validate_certs"]);
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
            Console.WriteLine($"WinPackage Module Execute");
            Console.ResetColor();
        }

        public string Provider { get; set; }

        public string Path { get; set; }

        public string ProductId { get; set; }

        public string Arguments { get; set; }

        public string State { get; set; }

        public string LogPath { get; set; }

        public string CreatesService { get; set; }

        public bool ValidateCerts { get; set; }

        public override string ToString()
        {
            return $"{ModuleName}:\n" +
                   $"    provider: {Provider}\n" +
                   $"    path: {Path}\n" +
                   $"    product_id: {ProductId}\n" +
                   $"    state: {State}\n" +
                   $"    log_path: {LogPath}\n" +
                   $"    creates_service: {CreatesService}\n" +
                   $"    validate_certs: {ValidateCerts}";
        }
    }
}
