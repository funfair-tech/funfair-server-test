using System;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Common.Mocks.Extensions;

public static partial class LogExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Hello World. It's {now}")]
    public static partial void LogHelloWorld(this ILogger logger, DateTimeOffset now);
}
