using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinUnzipModule: Unzips a file on a remote windows machine
    ///   community.windows.win_unzip:
    ///     src: File to be unzipped (provide absolute path).
    ///     dest: Destination of zip file (provide absolute path of directory). If it does not exist, the directory will be created.
    ///     creates: If this file or directory exists the specified src will not be extracted.
    ///     recurse: Recursively expand zipped files within the src file. Default is false.
    ///     password: If a zip file is encrypted with password. Passing a value to a password parameter requires the PSCX module to be installed.
    ///     delete_archive: Remove the zip file, after unzipping. Default is false.
    /// </summary>
    internal class WinUnzipModule : ModuleBase
    {
        private WinUnzipModule() : base(CommunityModuleNames.WIN_UNZIP)
        {
        }

        public static WinUnzipModule Create(Dictionary<object, object> dict)
        {
            var module = new WinUnzipModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("src"))
            {
                module.Source = dict["src"].ToString();
            }

            if (dict.ContainsKey("dest"))
            {
                module.Destination = dict["dest"].ToString();
            }

            if (dict.ContainsKey("creates"))
            {
                module.Creates = dict["creates"].ToString();
            }

            if (dict.ContainsKey("recurse"))
            {
                module.Recursive = dict["recurse"].ToString() == "yes";
            }

            if (dict.ContainsKey("password"))
            {
                module.Password = dict["password"].ToString();
            }

            if (dict.ContainsKey("delete_archive"))
            {
                module.DeleteArchive = dict["delete_archive"].ToString() == "yes";
            }

            return module;
        }

        public string Creates { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Password { get; set; }

        public bool Recursive { get; set; }

        public bool DeleteArchive { get; set; }

        public override void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Executing CommunityWinUnzipModule");
            Console.ResetColor();
        }

        public override bool Validate()
        {
            return true;
        }

        public override string ToString()
        {
            return $"Source: {Source}, Destination: {Destination}, Creates: {Creates}, Recursive: {Recursive}, Password: {Password}, DeleteArchive: {DeleteArchive}";
        }
    }

}
