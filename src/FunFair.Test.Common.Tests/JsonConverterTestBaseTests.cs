using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests
{
    internal enum ModelColor
    {
        Red,
        Blue
    }

    public sealed class Model : IEquatable<Model>
    {
        internal ModelColor? Value { get; set; }

        public bool Equals(Model other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Model other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return (this.Value != null ? this.Value.GetHashCode() : 0);
        }

        public static bool operator ==(Model left, Model right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Model left, Model right)
        {
            return !Equals(left, right);
        }
    }

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

    public class JsonConverterTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
    {
        public JsonConverterTestBaseTests(ITestOutputHelper output)
            : base(output)
        {

        }

        protected override Model CreateInstance()
        {
            return new Model() { Value = ModelColor.Blue };
        }
    }
}
