namespace Common.Models
{
    public class RunType : StringEnumeration
    {
        public static RunType OnDemand = new RunType("ONDEMAND", "Running process on demand");
        public static RunType Always = new RunType("ALWAYS", "Running process always");

        public RunType(string id, string name) : base(id, name)
        {
        }
    }
}