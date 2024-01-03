using System.Collections.Generic;

namespace ToolManager.Models
{
    public class PropertyConfigAction
    {
        public string CommentSymbol { get; set; }

        public string Source { get; set; }

        public List<PropertyValue> PropertyValues { get; set; }
    }
}