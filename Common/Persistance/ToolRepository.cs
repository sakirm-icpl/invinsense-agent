using System.Collections.Generic;

namespace Common.Persistance
{
    public class ToolRepository : BaseRepository<ToolDetail>
    {
        protected override string CollectionName => "tool_details";

        public ToolRepository()
        {
            
        }

        public IEnumerable<ToolDetail> GetTools()
        {
            using (var db = GetDatabase())
            {
                var col = GetCollection(db);
                return col.FindAll();
            }
        }
    }
}
