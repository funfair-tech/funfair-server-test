using FunFair.Test.Common.Tests.Mocks.JsonConverter;

namespace FunFair.Test.Common.Tests
{
    public sealed class JsonConverterTestBaseTests : JsonConverterTestBase<ModelConverter, Model>
    {
        public JsonConverterTestBaseTests(ITestOutputHelper output)
            : base(output)
        {
        }

        protected override string InvalidValue { get; } = "banana";

        protected override Model CreateInstance()
        {
            return new Model {Value = ModelColor.BLUE};
        }
    }
}