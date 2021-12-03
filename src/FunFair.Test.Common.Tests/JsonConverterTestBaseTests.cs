using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Common.Tests.Mocks.Converters;
using FunFair.Test.Common.Tests.Mocks.Converters.JsonConverter;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
public sealed class JsonConverterTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
{
    public JsonConverterTestBaseTests(ITestOutputHelper output)
        : base(output)
    {
    }

    protected override string InvalidValue { get; } = "banana";

    protected override Model CreateInstance()
    {
        return new() { Value = ModelColor.BLUE };
    }
}