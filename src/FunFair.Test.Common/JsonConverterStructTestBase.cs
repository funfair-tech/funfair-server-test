using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace FunFair.Test.Common;

public abstract class JsonConverterStructTestBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConverter,
    TObject
> : LoggingTestBase
    where TConverter : JsonConverter<TObject>, new()
    where TObject : struct
{
    private readonly JsonSerializerOptions _options;

    protected JsonConverterStructTestBase(ITestOutputHelper output)
        : base(output)
    {
        JsonConverter converter = new TConverter();
        this._options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { converter },
        };
    }

    protected virtual string InvalidValue { get; } = Guid.NewGuid().ToString();

    protected abstract TObject CreateInstance();

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

    [Fact]
    public void Serializes()
    {
        TObject instance = this.CreateInstance();

        Model sourceModel = new() { Value = instance };

        string doc = JsonSerializer.Serialize(value: sourceModel, options: this._options);
        Assert.NotEmpty(doc);

        this.Output.WriteLine($"Serialized model as: {doc}");
    }

    [Fact]
    public void ShouldNotDeserialize()
    {
        string doc = JsonSerializer.Serialize(new { value = this.InvalidValue }, options: this._options);

        this.Output.WriteLine($"Serialized model as: {doc}");

        JsonException exception = Assert.Throws<JsonException>(testCode: () => this.DeserializeDoc(doc));
        UnusedVariable(exception);
    }

    private Model DeserializeDoc(string doc)
    {
        return JsonSerializer.Deserialize<Model>(json: doc, options: this._options);
    }

    private readonly struct Model
    {
        [SuppressMessage(
            category: "ReSharper",
            checkId: "UnusedAutoPropertyAccessor.Local",
            Justification = "For unit tests"
        )]
        public TObject Value { get; init; }
    }
}
