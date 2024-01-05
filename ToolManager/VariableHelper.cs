using Common.RegistryHelpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ToolManager
{
    public static class VariableHelper
    {
        private static readonly string REGEX_PATTERN = @"\{\{(.+?)\}\}";

        public static string[] PrepareArgs(string[] input)
        {
            var args = new List<string>();
            foreach (var arg in input)
            {
                Match match = Regex.Match(arg, REGEX_PATTERN);
                if (match.Success)
                {
                    string content = match.Groups[1].Value;
                    var value = WinRegistryHelper.GetPropertyByTemplate(content);
                    args.Add(arg.Replace(match.Value, value));
                }
                else
                {
                    args.Add(arg);
                }
            }
            return args.ToArray();
        }
    }
}