using System;
using System.Collections.Generic;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// PingModule: Try to connect to host, verify a usable python and return pong on success
    ///   ansible.builtin.ping:
    ///     data: data to return. Optional, default is "pong"
    /// </summary>
    internal sealed class PingModule : ModuleBase
    {
        private PingModule() : base(CoreModuleNames.WIN_PING)
        {
            Data = "pong";
        }

        public static PingModule Create(Dictionary<object, object> dict)
        {
            var module = new PingModule();

            if (dict == null)
            {
                return module;
            }

            if (dict.ContainsKey("data"))
            {
                module.Data = dict["data"].ToString();
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
            Console.WriteLine(Data);
            Console.ResetColor();
        }

        public string Data { get; set; }

        public override string ToString()
        {
            return $"{ModuleName}:\n    data: {Data}";
        }
    }
}
