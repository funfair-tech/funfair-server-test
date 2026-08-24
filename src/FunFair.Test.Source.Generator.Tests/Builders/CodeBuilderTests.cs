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

    [Fact]
    public void AppendLine_WithWhitespaceText_AddsBlankLineInstead()
    {
        CodeBuilder builder = new CodeBuilder().AppendLine("   ");

        Assert.Equal(expected: Environment.NewLine, actual: builder.Text.ToString());
    }

    [Fact]
    public void AppendLine_WithEmptyText_AddsBlankLineInstead()
    {
        CodeBuilder builder = new CodeBuilder().AppendLine(string.Empty);

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
