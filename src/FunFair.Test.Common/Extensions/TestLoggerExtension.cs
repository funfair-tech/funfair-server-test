using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common.Extensions;

public static class TestLoggerExtension
{
    public static void Received(this ILogger logger, LogLevel logLevel, string message, int received = 1)
    {
        int count = CountLogCalls(logLevel: logLevel, message: message, logger: logger);
        Assert.Equal(received, count);
    }

    public static void DidNotReceive(this ILogger logger, LogLevel logLevel, string message)
    {
        int count = CountLogCalls(logLevel: logLevel, message: message, logger: logger);
        Assert.Equal(0, count);
    }

    private static int CountLogCalls(LogLevel logLevel, string message, ILogger logger)
    {
        return logger
            .ReceivedCalls()
            .Count(call =>
            {
                if (!string.Equals(a: call.GetMethodInfo().Name, b: "Log", comparisonType: StringComparison.Ordinal))
                {
                    return false;
                }

                object?[] args = call.GetArguments();

                if (args.Length < 3 || args[0] is not LogLevel level || level != logLevel)
                {
                    return false;
                }

                object? stateArg = args[2];

                return stateArg is not null
                    && string.Equals(a: stateArg.ToString(), b: message, comparisonType: StringComparison.Ordinal);
            });
    }
}
