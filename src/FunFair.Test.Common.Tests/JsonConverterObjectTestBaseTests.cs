using System;
using System.Reflection;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks.Converters;
using FunFair.Test.Common.Mocks.Converters.JsonConverter;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class JsonConverterObjectTestBaseTests : JsonConverterObjectTestBase<ModelConverter, Model>
{
    public JsonConverterObjectTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override string InvalidValue { get; } = "banana";

    protected override Model CreateInstance()
    {
        return new() { Value = ModelColor.BLUE };
    }

    public static TheoryData<string, Action<JsonConverterObjectTestBaseTests>> BaseCaseData()
    {
        return BuildDispatcherCases<JsonConverterObjectTestBaseTests>().ToTheoryData();
    }

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<JsonConverterObjectTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }

    [Fact]
    public void OptionsAreSharedAcrossInstancesOfSameClosedType()
    {
        FieldInfo field = AssertReallyNotNull(
            typeof(JsonConverterObjectTestBase<ModelConverter, Model>).GetField(
                name: "_options",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic
            )
        );

        using JsonConverterObjectTestBaseTests other = new(this.Output);

        Assert.Same(expected: field.GetValue(this), actual: field.GetValue(other));
    }
}
