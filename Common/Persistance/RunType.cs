namespace Common.Persistence
{
    public class RunType : StringEnumeration
    {
        public RunType(string id, string name) : base(id, name)
        {
        }

        public static RunType OnDemand = new RunType("ONDEMAND", "Running process on demand");
        public static RunType Always = new RunType("ALWAYS", "Running process always");
    }
}
