using System.Collections.Generic;

namespace Tensible.Models
{
    internal class HostGroup
    {
        public string Name { get; set; }
        public List<Host> Hosts { get; set; }
        public Dictionary<string, string> Vars { get; set; }
    }

}
