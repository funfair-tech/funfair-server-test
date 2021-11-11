using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using FunFair.Test.Common.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Base class for JSON Converters.
    /// </summary>
    /// <typeparam name="TConverter">The type of converter</typeparam>
    /// <typeparam name="TObject">The object being converted.</typeparam>
    public abstract class JsonConverterTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConverter, TObject> : LoggingTestBase
        where TConverter : JsonConverter<TObject>, new() where TObject : class
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">The test output.</param>
        protected JsonConverterTestBase(ITestOutputHelper output)
            : base(output)
        {
            JsonConverter converter = new TConverter();
            this._options = new JsonSerializerOptions
                            {
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                PropertyNameCaseInsensitive = false,
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                Converters = { converter }
                            };
        }

        /// <summary>
        ///     Gets an value that the converter will fail to convert to an object.
        /// </summary>
        protected virtual string InvalidValue { get; } = Guid.NewGuid()
                                                             .ToString();

        /// <summary>
        ///     Creates an instance of the object.
        /// </summary>
        /// <returns>The test object.</returns>
        protected abstract TObject CreateInstance();

        /// <summary>
        ///     Found-Trips conversion to and from JSON ensuring that the objects are the same.
        /// </summary>
        [Fact]
        public void RoundTrip()
        {
            TObject instance = this.CreateInstance();

            Model sourceModel = new() { Value = instance };

            // banana!
            string doc = JsonSerializer.Serialize(value: sourceModel, options: this._options);
            Assert.NotEmpty(doc);

            this.Output.WriteLine($"Serialized model as: {doc}");

            Model? targetModel = JsonSerializer.Deserialize<Model>(json: doc, options: this._options);
            Model mod = AssertReallyNotNull(targetModel);

            Assert.Equal(expected: sourceModel, actual: mod);
        }

        /// <summary>
        ///     Tests that the serialization works.
        /// </summary>
        [Fact]
        public void Serializes()
        {
            TObject instance = this.CreateInstance();

            Model sourceModel = new() { Value = instance };

            string doc = JsonSerializer.Serialize(value: sourceModel, options: this._options);
            Assert.NotEmpty(doc);

            this.Output.WriteLine($"Serialized model as: {doc}");
        }

        /// <summary>
        ///     Tests that a value that shouldn't serialize
        /// </summary>
        [Fact]
        public void ShouldNotDeserialize()
        {
            string doc = JsonSerializer.Serialize(new { value = this.InvalidValue }, options: this._options);

            this.Output.WriteLine($"Serialized model as: {doc}");

            Assert.Throws<JsonException>(testCode: () => JsonSerializer.Deserialize<Model>(json: doc, options: this._options));
        }

        private sealed class Model : IEquatable<Model>
        {
            public TObject? Value { get; init; }

            public bool Equals(Model? other)
            {
                return AreEqual(this, m2: other);
            }

            public override bool Equals(object? obj)
            {
                return AreEqual(this, obj as Model);
            }

            public override int GetHashCode()
            {
                return this.Value != null
                    ? this.Value.GetHashCode()
                    : 0;
            }

            public static bool operator ==(Model? left, Model? right)
            {
                return AreEqual(m1: left, m2: right);
            }

            public static bool operator !=(Model? left, Model? right)
            {
                return !AreEqual(m1: left, m2: right);
            }

            private static bool AreEqual(Model? m1, Model? m2)
            {
                return ReferenceObjectHelpers.AreEqual(left: m1, right: m2, eq: (l, r) => AreValuesEqual(o1: l.Value, o2: r.Value));
            }

            private static bool AreValuesEqual(TObject? o1, TObject? o2)
            {
                return ReferenceObjectHelpers.AreEqual(left: o1, right: o2, eq: (l, r) => l.Equals(r));
            }
        }
    }
}