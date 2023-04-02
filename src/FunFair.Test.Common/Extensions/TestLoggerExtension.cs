using System;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FunFair.Test.Common.Extensions;

public static class TestLoggerExtension
{
    public static void Received(this ILogger logger, LogLevel logLevel, string message, int received = 1)
    {
        logger.Received(received)
              .Log(logLevel: logLevel, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == message), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }

    public static void DidNotReceive(this ILogger logger, LogLevel logLevel, string message)
    {
        logger.DidNotReceive()
              .Log(logLevel: logLevel, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == message), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }
}