using LiteDB;
using Serilog;
using System.IO;
using System.Reflection;

namespace Common.Persistance
{
    public abstract class BaseRepository<T>
    {
        protected static readonly ILogger Logger = Log.ForContext(typeof(T));

        public LiteDatabase GetDatabase()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "db");
            return new LiteDatabase(path);
        }

        public bool IsExists()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "db");
            return File.Exists(path);
        }

        protected abstract string CollectionName { get; }

        protected ILiteCollection<T> GetCollection(LiteDatabase db)
        {
            return db.GetCollection<T>();
        }
    }
}
