using FunFair.Test.Common.Tests.Mocks.Converters;
using FunFair.Test.Common.Tests.Mocks.Converters.TypeConverter;
using Xunit;

namespace FunFair.Test.Common.Tests
{
    public sealed class TypeConverterTestBaseTests : TypeConverterTestBase<ModelTypeConverter, Model>
    {
        [Theory]
        [InlineData("RED")]
        [InlineData("BLUE")]
        public void ShouldConvert(string value)
        {
            object? convertedValue = this.GetConvertedValue(rawValue: value);

            Assert.NotNull(convertedValue);
        }

        [Theory]
        [InlineData("not")]
        [InlineData(null)]
        public void Should_NotConvertDueInvalidValue(string value)
        {
            object? convertedValue = this.GetConvertedValue(rawValue: value);

            Assert.Null(convertedValue);
        }
    }
}