using Common.Utils;
using LiteDB;
using Serilog;
using System.IO;

namespace Common.Persistance
{
    public abstract class BaseRepository<T>
    {
        protected static readonly ILogger Logger = Log.ForContext(typeof(T));

        public LiteDatabase GetDatabase()
        {
            return new LiteDatabase($"Filename={CommonUtils.DbPath};Connection=Shared") { UtcDate = true };
        }

        public bool IsExists()
        {
            return File.Exists(CommonUtils.DbPath);
        }

        protected abstract string CollectionName { get; }

        protected ILiteCollection<T> GetCollection(LiteDatabase db)
        {
            return db.GetCollection<T>();
        }
    }
}
