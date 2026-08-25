using System;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks.Converters;
using FunFair.Test.Common.Mocks.Converters.JsonConverter;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class JsonConverterTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
{
    public JsonConverterTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override string InvalidValue { get; } = "banana";

    protected override Model CreateInstance()
    {
        return new() { Value = ModelColor.BLUE };
    }

    public static TheoryData<string, Action<JsonConverterTestBaseTests>> BaseCaseData() =>
        new()
        {
            { nameof(RoundTrip), t => t.RoundTrip() },
            { nameof(Serializes), t => t.Serializes() },
            { nameof(ShouldNotDeserialize), t => t.ShouldNotDeserialize() },
        };

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<JsonConverterTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
