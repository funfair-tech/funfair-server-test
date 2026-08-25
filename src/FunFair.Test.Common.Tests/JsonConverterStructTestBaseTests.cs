using System;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks.Converters;
using FunFair.Test.Common.Mocks.Converters.JsonConverter;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class JsonConverterStructTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
{
    public JsonConverterStructTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override string InvalidValue { get; } = "banana";

    protected override Model CreateInstance()
    {
        return new() { Value = ModelColor.BLUE };
    }

    public static TheoryData<string, Action<JsonConverterStructTestBaseTests>> BaseCaseData() =>
        new()
        {
            { nameof(RoundTrip), t => t.RoundTrip() },
            { nameof(Serializes), t => t.Serializes() },
            { nameof(ShouldNotDeserialize), t => t.ShouldNotDeserialize() },
        };

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<JsonConverterStructTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
