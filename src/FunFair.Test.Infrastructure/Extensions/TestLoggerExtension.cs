using System;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FunFair.Test.Infrastructure.Extensions;

public static class TestLoggerExtension
{
    public static void Received(this ILogger logger, LogLevel logLevel, string message, int received = 1)
    {
        EventId eventId = Arg.Any<EventId>();
        object state = Arg.Is<object>(o => StringComparer.Ordinal.Equals(x: o.ToString(), y: message));
        Exception? exception = Arg.Any<Exception?>();
        Func<object, Exception?, string> formatter = Arg.Any<Func<object, Exception?, string>>();

        logger.Received(received).Log(logLevel: logLevel, eventId, state, exception, formatter);
    }

    public static void DidNotReceive(this ILogger logger, LogLevel logLevel, string message)
    {
        EventId eventId = Arg.Any<EventId>();
        object state = Arg.Is<object>(o => StringComparer.Ordinal.Equals(x: o.ToString(), y: message));
        Exception? exception = Arg.Any<Exception?>();
        Func<object, Exception?, string> formatter = Arg.Any<Func<object, Exception?, string>>();

        logger.DidNotReceive().Log(logLevel: logLevel, eventId, state, exception, formatter);
    }
}
