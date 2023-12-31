using Common.Core;

namespace ToolManager.Models
{
    public class ToolGroup : IntEnumeration
    {
        public static ToolGroup EndpointDetectionAndResponse = new ToolGroup(100, "ENDPOINT_DETECTION_AND_RESPONSE", "Endpoint Detection and Response");

        public static ToolGroup EndpointDeception = new ToolGroup(200, "ENDPOINT_DECEPTION", "Endpoint Deception");
        
        public static ToolGroup AdvanceTelemetry = new ToolGroup(300, "ADVANCE_TELEMETRY", "Advance Telemetry");

        public static ToolGroup UserBehaviorAnalytics = new ToolGroup(400, "USER_BEHAVIOUR_ANALYTICS", "User Behavior Analytics");

        public static ToolGroup EndpointProtection = new ToolGroup(500, "ENDPOINT_PROTECTION", "Endpoint Protection");

        public static ToolGroup LateralMovementProtection = new ToolGroup(600, "LATERAL_MOVEMENT_PROTECTION", "Lateral Movement Protection");

        public readonly string GroupCode;

        public ToolGroup(int id, string name, string groupCode) : base(id, name)
        {
            GroupCode = groupCode;
        }
    }
}