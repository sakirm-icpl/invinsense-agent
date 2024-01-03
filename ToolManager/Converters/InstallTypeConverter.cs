using Newtonsoft.Json;
using System;
using ToolManager.Models;

namespace ToolManager.Converters
{
    public class InstallTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var toolGroupValue = (InstallType)value;
            writer.WriteValue(toolGroupValue.Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            return InstallType.FromId<InstallType>(value);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
