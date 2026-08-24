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
}
