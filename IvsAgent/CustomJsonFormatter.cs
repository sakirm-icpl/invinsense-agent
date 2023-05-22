using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using System.IO;

namespace IvsAgent
{
    public class CustomJsonFormatter : ITextFormatter
    {
        private const string TimestampPropertyName = "@timestamp";
        private const string LevelPropertyName = "level";
        private const string MessagePropertyName = "message";
        private const string ExceptionPropertyName = "exception";

        public void Format(LogEvent logEvent, TextWriter output)
        {
            using (var jsonWriter = new JsonTextWriter(output))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName(TimestampPropertyName);
                jsonWriter.WriteValue(logEvent.Timestamp.ToString("o"));

                jsonWriter.WritePropertyName(LevelPropertyName);
                jsonWriter.WriteValue(logEvent.Level.ToString());

                jsonWriter.WritePropertyName(MessagePropertyName);
                jsonWriter.WriteValue(logEvent.MessageTemplate.Text);

                if (logEvent.Exception != null)
                {
                    jsonWriter.WritePropertyName(ExceptionPropertyName);
                    jsonWriter.WriteValue(logEvent.Exception.ToString());
                }

                jsonWriter.WriteEndObject();
            }
        }
    }
}
