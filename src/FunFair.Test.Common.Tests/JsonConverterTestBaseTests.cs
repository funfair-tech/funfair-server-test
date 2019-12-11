using FunFair.Test.Common.Tests.Mocks.JsonConverter;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests
{
    public sealed class JsonConverterTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
    {
        protected override string InvalidValue { get; } = "banana";

        public JsonConverterTestBaseTests(ITestOutputHelper output)
            : base(output)
        {

        }

        protected override Model CreateInstance()
        {
            return new Model() { Value = ModelColor.BLUE };
        }
    }
}
