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
    [InlineData("t => t.FactOne().ToString(System.Globalization.CultureInfo.InvariantCulture)", "ToString")]
    [InlineData("Example()", "Example")]
    [InlineData("t => Example()", "Example")]
    [InlineData("t => t?.Example()", "Example")]
    [InlineData("t => t . Example ()", "Example")]
    [InlineData("t => t.Example<int>()", "Example")]
    [InlineData("t => t.Example<List<int>>()", "Example")]
    [InlineData("t => t.Example<Dictionary<string, List<int>>>()", "Example")]
    [InlineData("t => t.Check(new Foo())", "Check")]
    [InlineData("t => t.Check(x.Get())", "Check")]
    [InlineData("t => t.Check(t.Items[0].Get())", "Check")]
    [InlineData("t => t.Check(x => x.Get())", "Check")]
    [InlineData("t => t.Check(x => { x.Get(); })", "Check")]
    [InlineData("t => t.Check(new Foo { Bar = 1 })", "Check")]
    [InlineData("t => t.Check(\"a(b\")", "Check")]
    [InlineData("t => t.Check(\"a\\\"(b\")", "Check")]
    [InlineData("t => t.Check(@\"a\"\"(b\")", "Check")]
    [InlineData("t => t.Check('(')", "Check")]
    [InlineData("t => t.Check('\\'')", "Check")]
    [InlineData("t => t.Check(a / b)", "Check")]
    [InlineData("(t) => t.Example()", "Example")]
    [InlineData("static t => t.Example()", "Example")]
    [InlineData("static (t) => t.Example()", "Example")]
    [InlineData("(Foo t) => t.Example()", "Example")]
    [InlineData("async t => await t.ExampleAsync()", "ExampleAsync")]
    [InlineData("static async (t) => await t.ExampleAsync()", "ExampleAsync")]
    [InlineData("t => { t.Example(); }", "Example")]
    [InlineData("t => {t.Example() ;}", "Example")]
    public static void TryGetCaseNameReturnsTheOutermostCallsMethodName(string expressionText, string expected)
    {
        bool found = DispatcherCaseName.TryGetCaseName(expressionText: expressionText, out string name);

        Assert.True(condition: found, userMessage: "Should have found a case name");
        Assert.Equal(expected: expected, actual: name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("t => t.Example")]
    [InlineData("_ => { }")]
    [InlineData("(t) => { }")]
    [InlineData("static (t) => { }")]
    [InlineData("t => { ; }")]
    [InlineData("t => { t.Example() }")]
    [InlineData("t => { t.First(); t.Second(); }")]
    [InlineData("t => { { t.Example(); } }")]
    [InlineData("t => t.Example() + 1")]
    [InlineData("t => (t.Example)()")]
    [InlineData("t => new Foo()")]
    [InlineData("t => t.Example>()")]
    [InlineData("t => t.Example(")]
    [InlineData("t => t.Example())")]
    [InlineData("t => t.Check(\"a)")]
    [InlineData("t => t.Check('a)")]
    [InlineData("t => t.Check($\"a{b}\")")]
    [InlineData("t => t.Check($@\"a{b}\")")]
    [InlineData("t => t.Check(@$\"a{b}\")")]
    [InlineData("t => t.Check(\"\"\"a(b\"\"\")")]
    [InlineData("t => t.Check(/* ( */ 1)")]
    [InlineData("t => t.Check(1) // )")]
    public static void TryGetCaseNameReturnsFalseWhenThereIsNoConfidentlyParsedCall(string expressionText)
    {
        bool found = DispatcherCaseName.TryGetCaseName(expressionText: expressionText, out string name);

        Assert.False(condition: found, userMessage: "Should not have found a case name");
        Assert.Equal(expected: string.Empty, actual: name);
    }
}
