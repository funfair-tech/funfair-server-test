using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FunFair.Test.Common.Tests.Mocks.JsonConverter
{
    public sealed class ModelConverter : JsonConverter<Model>
    {
        public override Model Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string source = reader.GetString();

            if (!Enum.TryParse(source, out ModelColor color))
            {
                throw new JsonException("Unknown Color");
            }

            return new Model
                   {
                       Value = color
                   };
        }

        public override void Write(Utf8JsonWriter writer, Model value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
    }
}