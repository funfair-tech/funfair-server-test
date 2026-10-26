using FunFair.Test.Shared;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class DispatcherCaseNameTests : TestBase
{
    // CA1822: these [Theory] methods use only their parameters, so the analyzer requires static rather than the
    // TestBase-instance convention the [Fact] methods follow.
    [Theory]
    [InlineData("t => t.Example()", "Example")]
    [InlineData("t => t.Example(1, 2)", "Example")]
    [InlineData("t => t.Inner.Example()", "Example")]
    [InlineData("t => t.First().Second()", "Second")]
    [InlineData("Example()", "Example")]
    [InlineData("t => t.Example<int>()", "Example<int>")]
    public static void TryGetCaseNameReturnsTheIdentifierBeforeTheLastCall(string expressionText, string expected)
    {
        bool found = DispatcherCaseName.TryGetCaseName(expressionText: expressionText, out string name);

        Assert.True(condition: found, userMessage: "Should have found a case name");
        Assert.Equal(expected: expected, actual: name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("t => t.Example")]
    [InlineData("_ => { }")]
    public static void TryGetCaseNameReturnsFalseWhenThereIsNoCall(string expressionText)
    {
        bool found = DispatcherCaseName.TryGetCaseName(expressionText: expressionText, out string name);

        Assert.False(condition: found, userMessage: "Should not have found a case name");
        Assert.Equal(expected: string.Empty, actual: name);
    }
}
