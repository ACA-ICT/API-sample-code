using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACA.SampleCode.ApiClients.ConsoleNET5.Authorization
{
    public class TokenExpireDateJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.UtcNow.AddSeconds(reader.GetInt32());
        }

        public override void Write(Utf8JsonWriter writer , DateTime dateTimeValue, JsonSerializerOptions options)
        {
            var timeSpan = dateTimeValue - DateTime.UtcNow;
            var seconds = (int) timeSpan.TotalSeconds;
            writer.WriteStringValue(seconds.ToString());
        }
    }
}
