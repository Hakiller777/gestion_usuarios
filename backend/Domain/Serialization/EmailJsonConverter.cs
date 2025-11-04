using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using backend.Domain.ValueObjects;

namespace backend.Domain.Serialization
{
    public sealed class EmailJsonConverter : JsonConverter<Email>
    {
        public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str is null) throw new JsonException("Email value is null");
            return Email.Create(str);
        }

        public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
