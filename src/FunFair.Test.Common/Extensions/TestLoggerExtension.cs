using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FunFair.Test.Common.Extensions
{
    /// <summary>
    ///     Test logger extension
    /// </summary>    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public static class TestLoggerExtension
    {
        /// <summary>
        ///     Check if error message is received while testing
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Message</param>
        /// <param name="received">Received amount</param>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public static void Received(this ILogger logger, LogLevel logLevel, string message, int received = 1)
        {
            logger.Received(received)
                  .Log(logLevel: logLevel, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == message), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception, string>>());
        }

        /// <summary>
        ///     Check if error message is not received while testing
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Message</param>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public static void DidNotReceive(this ILogger logger, LogLevel logLevel, string message)
        {
            logger.DidNotReceive()
                  .Log(logLevel: logLevel, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == message), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception, string>>());
        }
    }
}
