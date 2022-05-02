using System.Linq;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class EquatableObjectTestBaseTests : EquatableObjectTestBase<string>
{
    public EquatableObjectTestBaseTests()
        : base(zeroObject: string.Empty,
               value1: "Hello",
               new("olleH".Reverse()
                          .ToArray()))
    {
    }

    protected override bool OperatorEquals(string? x, string? y)
    {
        return x == y;
    }

    protected override bool OperatorNotEquals(string? x, string? y)
    {
        return x != y;
    }

    [Fact]
    public void Test()
    {
        Assert.Equal(expected: this.Value1, actual: this.Value1Alias);
    }
}