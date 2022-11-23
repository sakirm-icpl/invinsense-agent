
namespace Common.Utils
{
    public static class SerializationExtension
    {
        public static Newtonsoft.Json.JsonSerializerSettings DefaultOptions
        {
            get
            {
                var options = new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                };

                return options;
            }
        }
    }
}
