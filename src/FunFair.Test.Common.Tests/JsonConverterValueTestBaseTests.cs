using System;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks.Converters;
using FunFair.Test.Common.Mocks.Converters.JsonConverter;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class JsonConverterValueTestBaseTests : JsonConverterValueTestBase<ModelValueConverter, ModelValue>
{
    public JsonConverterValueTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override string InvalidValue { get; } = "banana";

    protected override ModelValue CreateInstance()
    {
        return new(ModelColor.BLUE);
    }

    public static TheoryData<string, Action<JsonConverterValueTestBaseTests>> BaseCaseData() =>
        BuildDispatcherCases<JsonConverterValueTestBaseTests>().ToTheoryData();

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<JsonConverterValueTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
