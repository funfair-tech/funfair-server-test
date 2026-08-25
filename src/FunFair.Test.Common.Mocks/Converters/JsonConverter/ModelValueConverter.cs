using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FunFair.Test.Common.Mocks.Converters.JsonConverter;

public sealed class ModelValueConverter : JsonConverter<ModelValue>
{
    public override ModelValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? source = reader.GetString();

        if (!Enum.TryParse(value: source, out ModelColor color))
        {
            throw new JsonException(message: "Unknown Color");
        }

        return new(color);
    }

    public override void Write(Utf8JsonWriter writer, ModelValue value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.GetName());
    }
}
