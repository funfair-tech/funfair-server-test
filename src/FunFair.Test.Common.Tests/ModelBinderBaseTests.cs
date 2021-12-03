using System.Threading.Tasks;
using FunFair.Test.Common.Tests.Mocks.Converters;
using FunFair.Test.Common.Tests.Mocks.Converters.Binders;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class ModelBinderBaseTests : ModelBinderTestsBase<ModelBinder, ModelColor>
{
    public ModelBinderBaseTests()
        : base(new())
    {
    }

    [Theory]
    [InlineData("ReD", ModelColor.RED)]
    [InlineData("BLUE", ModelColor.BLUE)]
    public Task ShouldConvertAsync(string value, ModelColor expected)
    {
        return this.MustConvertAsync(value: value, expected: expected);
    }

    [Theory]
    [InlineData("Banana")]
    [InlineData("ORANGE")]
    public Task ShouldNotConvertAsync(string value)
    {
        return this.MustNotConvertAsync(value: value);
    }
}