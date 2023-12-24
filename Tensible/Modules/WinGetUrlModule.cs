using Tensible.Models;
using System;
using System.Collections.Generic;

namespace Tensible.Modules
{
    /// <summary>
    /// WinGetUrlModule: Download earthrise.jpg to specified path only if modified
    ///  ansible.windows.win_get_url:
    ///     url: http url to download from
    ///     dest: local path to download to
    ///     force: download to tmp location and overwrite dest if different
    /// </summary>
    internal class WinGetUrlModule : ModuleBase
    {
        private WinGetUrlModule() : base(WinModuleNames.WIN_GET_URL)
        {
            //Applying force by default
            Force = false;
        }

        public string Url { get; set; }

        public string DestinationPath { get; set; }

        public bool Force { get; set; }

        public static WinGetUrlModule Create(Dictionary<object, object> dict)
        {
            var module = new WinGetUrlModule();

            if (dict == null)
            {
                throw new ArgumentNullException("Missing required fields: url, dest");
            }

            if (dict.ContainsKey("url"))
            {
                module.Url = dict["url"].ToString();
            }

            if (dict.ContainsKey("dest"))
            {
                module.DestinationPath = dict["dest"].ToString();
            }

            if (dict.ContainsKey("force"))
            {
                module.Force = dict["force"].ToString() == "yes";
            }

            return module;
        }

        public override void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("TensibleWinGetModule.Execute()");
            Console.ResetColor();
        }

        public override bool Validate()
        {
            return true;
        }

        public override string ToString()
        {
            return $"{ModuleName}:\n    url: {Url}\n    dest: {DestinationPath}\n    force: {Force}";
        }
    }
}
