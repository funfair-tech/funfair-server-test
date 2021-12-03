using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

public sealed class LoggingTestBaseTests : LoggingTestBase
{
    public LoggingTestBaseTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0005:Avoid DateTimeOffset.UtcNow", Justification = "Unit test")]
    public void OutputOutputs()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        try
        {
            this.Output.WriteLine($"Hello World. It's {now}");
        }
        catch // (Exception exception)
        {
            throw new FormatException("Twit");
        }
    }
}