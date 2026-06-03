using System;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Infrastructure.Tests.Extensions;

internal static partial class LogExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Hello World. It's {now}")]
    public static partial void LogHelloWorld(this ILogger logger, DateTimeOffset now);
}
