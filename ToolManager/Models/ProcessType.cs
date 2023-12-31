using Common.Core;

namespace Common.Persistence
{
    public class ProcessType : StringEnumeration
    {
        public static ProcessType Executable = new ProcessType("EXECUTABLE", "Executable");
        public static ProcessType Service = new ProcessType("SERVICE", "Service");

        public ProcessType(string id, string name) : base(id, name)
        {
        }
    }
}