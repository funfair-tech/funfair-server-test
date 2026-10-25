using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks.Converters;
using FunFair.Test.Common.Mocks.Converters.JsonConverter;
using Xunit;

namespace FunFair.Test.Common.Tests;

[SuppressMessage(
    category: "xUnit",
    checkId: "xUnit1072",
    Justification = "Shared test base classes are intentionally declared in a referenced assembly"
)]
public sealed class JsonConverterObjectTestBaseTests : JsonConverterObjectTestBase<ModelConverter, Model>
{
    public JsonConverterObjectTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override string InvalidValue { get; } = "banana";

    protected override Model CreateInstance()
    {
        return new() { Value = ModelColor.BLUE };
    }

    public static TheoryData<string, Action<JsonConverterObjectTestBaseTests>> BaseCaseData() =>
        BuildDispatcherCases<JsonConverterObjectTestBaseTests>().ToTheoryData();

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<JsonConverterObjectTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
