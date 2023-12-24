using Tensible.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tensible.Modules
{
    /// <summary>
    /// WinCopyModule: Copies files to remote locations on windows hosts
    ///   ansible.windows.win_copy:
    ///     src: Source file
    ///     dest: Destination file
    ///     remote_src: If true, it will search for src at originating/master machine
    ///     backup: backup required before copy
    /// </summary>
    internal class WinCopyModule : ModuleBase
    {
        private WinCopyModule() : base(WinModuleNames.WIN_COPY)
        {
            Backup = false;
            RemoteSource = false;
        }

        public string Source { get; set; }
        
        public string Destination { get; set; }

        public bool RemoteSource { get; set; }
        
        public bool Backup { get; set; }

        public override bool Validate()
        {
            return true;
        }

        public static WinCopyModule Create(Dictionary<object, object> dict)
        {
            var module = new WinCopyModule();

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

            if (dict.ContainsKey("remote_src"))
            {
                module.RemoteSource = Convert.ToBoolean(dict["remote_src"]);
            }

            if (dict.ContainsKey("backup"))
            {
                module.Backup = dict["backup"].ToString() == "yes";
            }

            return module;
        }

        public override void Execute()
        {
            try
            {
                if (Backup && File.Exists(Destination))
                {
                    string backupFile = $"{Destination}.bak";
                    File.Copy(Destination, backupFile, true);
                    Console.WriteLine($"Backup created: {backupFile}");
                }

                File.Copy(Source, Destination, true);
                Console.WriteLine($"File copied from {Source} to {Destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file copy: {ex.Message}");
            }
        }

        public override string ToString()
        {
            return $"{WinModuleNames.WIN_COPY}:\n" +
             $"    src: {Source}\n" +
             $"    dest: {Destination}\n" +
             $"    remote_src: {RemoteSource}\n" +
             $"    backup: {Backup}";
        }
    }
}
