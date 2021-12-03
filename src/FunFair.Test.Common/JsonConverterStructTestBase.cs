using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

/// <summary>
///     Base class for JSON Converters.
/// </summary>
/// <typeparam name="TConverter">The type of converter</typeparam>
/// <typeparam name="TObject">The object being converted.</typeparam>
public abstract class JsonConverterStructTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConverter, TObject> : LoggingTestBase
    where TConverter : JsonConverter<TObject>, new() where TObject : struct
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">The test output.</param>
    protected JsonConverterStructTestBase(ITestOutputHelper output)
        : base(output)
    {
        JsonConverter converter = new TConverter();
        this._options = new()
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

        Model targetModel = JsonSerializer.Deserialize<Model>(json: doc, options: this._options);

        Assert.Equal(expected: sourceModel, actual: targetModel);
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

        Assert.Throws<JsonException>(testCode: () => this.DeserializeDoc(doc));
    }

    private Model DeserializeDoc(string doc)
    {
        return JsonSerializer.Deserialize<Model>(json: doc, options: this._options);
    }

    private readonly struct Model
    {
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "For unit tests")]
        public TObject Value { get; init; }
    }
}