using System;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Common.Extensions;

internal static partial class LoggingExtensions
{
    public static void LogWaitingForDispose(this ILogger logger, Type type)
    {
        logger.LogWaitingForDispose(type.FullName!);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Waiting for dispose of {type}...")]
    static partial void LogWaitingForDispose(this ILogger logger, string type);
}