using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using FunFair.Test.Infrastructure.Helpers;
using Xunit;
using static FunFair.Test.Common.DispatcherCaseData;

namespace FunFair.Test.Common;

public abstract class JsonConverterValueTestBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConverter,
    TObject
> : LoggingTestBase
    where TConverter : JsonConverter<TObject>, new()
    where TObject : struct
{
    private readonly JsonSerializerOptions _options;

    protected JsonConverterValueTestBase(ITestOutputHelper output)
        : base(output)
    {
        JsonConverter converter = new TConverter();
        this._options = JsonOptions.CreateDefault(converter);
    }

    protected virtual string InvalidValue { get; } = Guid.NewGuid().ToString();

    protected abstract TObject CreateInstance();

    [Fact]
    public void RoundTrip()
    {
        TObject instance = this.CreateInstance();

        Model sourceModel = new() { Value = instance };

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

    // Single source of truth for the AOT dispatcher case table (see FunFair.Test.Source.Generator's
    // AotTestDispatcherAnalyzer, FTS002); see EquatableObjectTestBase<TObject>.BuildDispatcherCases for why this
    // must stay an ordinary static generic method rather than a [MemberData] provider itself.
    [SuppressMessage(
        category: "Microsoft.Design",
        checkId: "CA1000:Do not declare static members on generic types",
        Justification = "Not a [MemberData] provider itself - a shared helper closed leaf classes call via "
            + "JsonConverterValueTestBase<TConverter, TObject>.BuildDispatcherCases<TSelf>(), avoiding a "
            + "hand-copied case table per leaf"
    )]
    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
        where TSelf : JsonConverterValueTestBase<TConverter, TObject> =>
        [Case<TSelf>(t => t.RoundTrip()), Case<TSelf>(t => t.Serializes()), Case<TSelf>(t => t.ShouldNotDeserialize())];

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
