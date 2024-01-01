namespace Common.Persistence
{
    public readonly struct ToolName
    {
        private readonly string _toolName;

        public ToolName(string toolName)
        {
            _toolName = toolName;
        }

        public static ToolName OsQuery => new ToolName("osquery");

        public static ToolName Sysmon => new ToolName ("sysmon");

        public static ToolName Wazuh => new ToolName ("wazuh");

        public static implicit operator string(ToolName toolName)
        {
            return toolName.ToString();
        }

        public override string ToString()
        {
            return _toolName;
        }
    }
}