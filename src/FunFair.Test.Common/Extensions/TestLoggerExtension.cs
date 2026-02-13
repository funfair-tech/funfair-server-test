using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FunFair.Test.Common.Extensions;

public static class TestLoggerExtension
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1873: Evaluation of this argument may be expensive and unnecessary if logging is disabled",
        Justification = "This is a unit test assembly - not so worried about performance"
    )]
    public static void Received(this ILogger logger, LogLevel logLevel, string message, int received = 1)
    {
        logger
            .Received(received)
            .Log(
                logLevel: logLevel,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString() == message),
                Arg.Any<Exception?>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
    }

    [SuppressMessage(
        "Microsoft.Performance",
        "CA1873: Evaluation of this argument may be expensive and unnecessary if logging is disabled",
        Justification = "This is a unit test assembly - not so worried about performance"
    )]
    public static void DidNotReceive(this ILogger logger, LogLevel logLevel, string message)
    {
        logger
            .DidNotReceive()
            .Log(
                logLevel: logLevel,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString() == message),
                Arg.Any<Exception?>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
    }
}
