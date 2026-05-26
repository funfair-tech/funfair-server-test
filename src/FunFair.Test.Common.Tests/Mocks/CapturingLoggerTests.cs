using System;
using FunFair.Test.Common;
using FunFair.Test.Common.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Tests.Mocks;

public sealed class CapturingLoggerTests : TestBase
{
    [Fact]
    public void LogEntry_IsCaptured()
    {
        CapturingLogger<CapturingLoggerTests> logger = new();
        logger.Log(
            logLevel: LogLevel.Information,
            eventId: new EventId(1, "Test"),
            state: "hello",
            exception: null,
            formatter: (s, _) => s
        );

        Assert.Single(logger.Entries);
        Assert.Equal(expected: LogLevel.Information, actual: logger.Entries[0].Level);
        Assert.Equal(expected: "hello", actual: logger.Entries[0].Message);
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForAllLevels()
    {
        CapturingLogger<CapturingLoggerTests> logger = new();

        foreach (LogLevel level in Enum.GetValues<LogLevel>())
        {
            Assert.True(
                condition: logger.IsEnabled(level),
                userMessage: "IsEnabled should return true for all log levels"
            );
        }
    }

    [Fact]
    public void BeginScope_ReturnsNull()
    {
        CapturingLogger<CapturingLoggerTests> logger = new();
        Assert.Null(logger.BeginScope("scope"));
    }
}
