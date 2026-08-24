using System;
using FunFair.Test.Common;
using FunFair.Test.Source.Generator.Builders;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests.Builders;

public sealed class CodeBuilderTests : TestBase
{
    [Fact]
    public void AppendLine_AddsTextFollowedByNewLine()
    {
        CodeBuilder builder = new CodeBuilder().AppendLine("hello");

        Assert.Equal(expected: "hello" + Environment.NewLine, actual: builder.Text.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    // CA1822: unlike the [Fact] methods in this suite, this [Theory] method uses only its parameter and
    // CodeBuilder, so the analyzer requires static rather than the TestBase-instance convention.
    public static void AppendLine_WithEmptyOrWhitespaceText_AddsBlankLineInstead(string text)
    {
        CodeBuilder builder = new CodeBuilder().AppendLine(text);

        Assert.Equal(expected: Environment.NewLine, actual: builder.Text.ToString());
    }

    [Fact]
    public void AppendBlankLine_AddsNewLineOnly()
    {
        CodeBuilder builder = new CodeBuilder().AppendBlankLine();

        Assert.Equal(expected: Environment.NewLine, actual: builder.Text.ToString());
    }

    [Fact]
    public void MultipleAppends_AccumulateInOrder()
    {
        CodeBuilder builder = new CodeBuilder().AppendLine("first").AppendBlankLine().AppendLine("second");

        Assert.Equal(
            expected: "first" + Environment.NewLine + Environment.NewLine + "second" + Environment.NewLine,
            actual: builder.Text.ToString()
        );
    }

    [Fact]
    public void AppendLine_ReturnsSameInstanceForChaining()
    {
        CodeBuilder builder = new();

        CodeBuilder returned = builder.AppendLine("hello");

        Assert.Same(expected: builder, actual: returned);
    }
}
