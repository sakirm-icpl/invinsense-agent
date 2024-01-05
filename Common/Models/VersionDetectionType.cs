namespace Common.Models
{
    public class VersionDetectionType : StringEnumeration
    {
        public static VersionDetectionType Unknown = new VersionDetectionType("UNKNOWN", "Unknown");
        public static VersionDetectionType ProgramRegistry = new VersionDetectionType("PROGRAM_REGISTRY", "Program Registry");
        public static VersionDetectionType ServiceRegistry = new VersionDetectionType("SERVICE_REGISTRY", "Service Registry");

        public VersionDetectionType(string id, string name) : base(id, name)
        {

        }
    }
}