using Newtonsoft.Json;
using System;

namespace Common.Models.Converters
{
    public class VersionDetectionTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var versionDetectionTypeValue = (VersionDetectionType)value;
            writer.WriteValue(versionDetectionTypeValue.Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return VersionDetectionType.FromId<VersionDetectionType>(value);
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
