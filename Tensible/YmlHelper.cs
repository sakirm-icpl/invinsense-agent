using System;
using System.Collections.Generic;
using System.IO;
using Tensible.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tensible
{
    public class YmlHelper
    {
        internal static Dictionary<string, object> ReadVariableFile(string filePath)
        {
            try
            {
                var serializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var content = File.ReadAllText(filePath);
                var result = serializer.Deserialize<Dictionary<string, object>>(content);

                return result ?? new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading YAML file: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        internal static Playbook[] ReadPlaybook(string filePath)
        {
            try
            {
                var serializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var content = File.ReadAllText(filePath);
                var playbooks = serializer.Deserialize<List<Playbook>>(content);

                return playbooks.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading YAML file: {ex.Message}");
                return null;
            }
        }
    }
}
