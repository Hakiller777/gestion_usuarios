using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using backend.Domain.ValueObjects;

namespace backend.Presentation.Serialization
{
    public sealed class PasswordHashJsonConverter : JsonConverter<PasswordHash>
    {
        public override PasswordHash Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str is null) throw new JsonException("PasswordHash value is null");
            return PasswordHash.FromHashed(str);
        }

        public override void Write(Utf8JsonWriter writer, PasswordHash value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
