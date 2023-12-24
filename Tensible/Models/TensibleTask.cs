using Tensible.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tensible.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class TensibleTask
    {
        public TensibleTask(Dictionary<object, object> dict)
        {
            Name = dict["name"].ToString();

            if (dict.ContainsKey(CoreModuleNames.WIN_PING))
            {
                var val = dict[CoreModuleNames.WIN_PING] as Dictionary<object, object>;
                Module = PingModule.Create(val);
            }
            else if (dict.ContainsKey(CommunityModuleNames.WIN_UNZIP))
            {
                var val = dict[CommunityModuleNames.WIN_UNZIP] as Dictionary<object, object>;
                Module = WinUnzipModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_FILE))
            {
                var val = dict[WinModuleNames.WIN_FILE] as Dictionary<object, object>;
                Module = WinFileModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_COPY))
            {
                var val = dict[WinModuleNames.WIN_COPY] as Dictionary<object, object>;
                Module = WinCopyModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_GET_URL))
            {
                var val = dict[WinModuleNames.WIN_GET_URL] as Dictionary<object, object>;
                Module = WinGetUrlModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_SERVICE))
            {
                var val = dict[WinModuleNames.WIN_SERVICE] as Dictionary<object, object>;
                Module = WinServiceModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_PACKAGE))
            {
                var val = dict[WinModuleNames.WIN_PACKAGE] as Dictionary<object, object>;
                Module = WinPackageModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_REGEDIT))
            {
                var val = dict[WinModuleNames.WIN_REGEDIT] as Dictionary<object, object>;
                Module = WinRegeditModule.Create(val);
            }
            else if (dict.ContainsKey(WinModuleNames.WIN_USER_RIGHT))
            {
                var val = dict[WinModuleNames.WIN_USER_RIGHT] as Dictionary<object, object>;
                Module = WinUserRightModule.Create(val);
            }
            else
            {
                Console.WriteLine($"Step not implemented. {Name}");
            }
        }

        public string Name { get; }

        public ModuleBase Module { get; }

        public bool When()
        {
            return true;
        }

        public bool Validate()
        {
            return true;
        }

        public string Diff()
        {
            return "Not implemented";
        }

        public override string ToString()
        {
            return $"- name: {Name}\n  {Module}\n  when: {When()}\n  validate: {Validate()}\n  diff: {Diff()}";
        }
    }
}
