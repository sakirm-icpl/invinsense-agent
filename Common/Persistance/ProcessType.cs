namespace Common.Persistence
{
    public class ProcessType : StringEnumeration
    {
        public ProcessType(string id, string name) : base(id, name)
        {
        }

        public static ProcessType Executable = new ProcessType("EXECUTABLE", "Executable");
        public static ProcessType Service = new ProcessType("SERVICE", "Service");
    }
}
