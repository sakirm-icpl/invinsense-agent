using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tensible.Models
{
    public class Playbook
    {
        public string Name { get; set; }

        public string Hosts { get; set; }

        public string Connection { get; set; }

        // Making gather_facts nullable and default to true
        [YamlMember(Alias = "gather_facts", ApplyNamingConventions = false)]
        public string GatherFacts { get; set; }

        // Import variables from files
        [YamlMember(Alias = "vars_files", ApplyNamingConventions = false)]
        public List<object> VarsFiles { get; set; }

        public List<Dictionary<object, object>> Tasks { get; set; }

        public int TotalTasks => Tasks.Count;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Playbook: {Name}");
            sb.AppendLine($"Hosts: {Hosts}");
            sb.AppendLine($"Connection: {Connection}");
            sb.AppendLine($"GatherFacts: {GatherFacts}");
            sb.AppendLine($"VarsFiles: {string.Join(",", VarsFiles)}");
            sb.AppendLine($"Tasks: {Tasks.Count}");
            foreach (var item in Tasks)
            {
                foreach (var ele in item)
                {
                    sb.AppendLine($"{ele.Key}: {ele.Value}");
                }
            }

            return sb.ToString();
        }
    }

}
