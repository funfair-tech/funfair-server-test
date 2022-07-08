using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Common.Tests.Mocks;
using Microsoft.Extensions.Logging;
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
        catch (Exception exception)
        {
            throw new FormatException(message: "Twit", innerException: exception);
        }
    }

    [Fact]
    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0005:Avoid DateTimeOffset.UtcNow", Justification = "Unit test")]
    public void LoggingOutputs()
    {
        ILogger<LoggingTestBaseTests> logger = this.GetTypedLogger<LoggingTestBaseTests>();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        try
        {
            logger.LogError($"Hello World. It's {now}");
        }
        catch (Exception exception)
        {
            throw new FormatException(message: "Twit", innerException: exception);
        }
    }

    [Fact]
    public void MakeFaker()
    {
        List<ExampleObject> fake = MakeFake<ExampleObject>()
                                   .RuleFor(property: x => x.Name, setter: (f, _) => f.Company.Bs())
                                   .Generate(10);

        foreach (ExampleObject item in fake)
        {
            this.Output.WriteLine($"* {item.Name}");
        }
    }
}