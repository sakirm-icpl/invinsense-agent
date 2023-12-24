using System;
using System.Collections.Generic;
using System.Linq;
using Tensible.Models;

namespace Tensible.Modules
{
    /// <summary>
    /// WinUserRightModule:  Manage Windows User Rights
    ///   ansible.windows.win_user_right:
    ///     name: The name of the User Right as shown by the Constant Name value from https://learn.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/user-rights-assignment.
    ///     users: A list of users or groups to add/remove on the User Right.
    ///     action: add / remove / set
    /// </summary>
    internal class WinUserRightModule : ModuleBase
    {
        private WinUserRightModule(string module) : base(module)
        {
        }

        public static WinUserRightModule Create(Dictionary<object, object> dict)
        {
            var module = new WinUserRightModule(WinModuleNames.WIN_USER_RIGHT);

            if(dict.ContainsKey("name"))
            {
                module.Name = dict["name"].ToString();
            }

            if (dict.ContainsKey("users"))
            {
                var users = dict["users"] as List<object>;
                module.Users = users.OfType<string>().ToArray();
            }

            if (dict.ContainsKey("action"))
            {
                module.Action = dict["action"].ToString();
            }

            return module;
        }

        public string Name { get; private set; }

        public string[] Users { get; private set; }

        public string Action { get; private set; }

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
            return $"{ModuleName}:\n  name: {Name}\n  users: {Users}\n  action: {Action}";
        }
    }
}
