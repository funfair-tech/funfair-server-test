using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Helpers
{
    internal static class XUnitLoggingHelpers
    {
        [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "A mock of unit tests")]
        public static ILoggingBuilder AddXUnit(this ILoggingBuilder builder, ITestOutputHelper output)
        {
            builder.AddProvider(new XunitLoggerProvider(output));

            return builder;
        }

        private sealed class XunitLoggerProvider : ILoggerProvider
        {
            private readonly DateTimeOffset? _logStart;
            private readonly LogLevel _minLevel;
            private readonly ITestOutputHelper _output;

            public XunitLoggerProvider(ITestOutputHelper output)
                : this(output, LogLevel.Trace)
            {
            }

            public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel)
                : this(output, minLevel, logStart: null)
            {
            }

            public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel, DateTimeOffset? logStart)
            {
                this._output = output;
                this._minLevel = minLevel;
                this._logStart = logStart;
            }

            public ILogger CreateLogger(string categoryName)
            {
                return new XunitLogger(this._output, categoryName, this._minLevel, this._logStart);
            }

            public void Dispose()
            {
                // nothing to dispose
            }
        }

        private sealed class XunitLogger : ILogger
        {
            private static readonly string[] NewLineChars = {Environment.NewLine};
            private readonly string _category;
            private readonly LogLevel _minLogLevel;
            private readonly ITestOutputHelper _output;
            private DateTimeOffset? _logStart;

            public XunitLogger(ITestOutputHelper output, string category, LogLevel minLogLevel, DateTimeOffset? logStart)
            {
                this._minLogLevel = minLogLevel;
                this._category = category;
                this._output = output;
                this._logStart = logStart;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!this.IsEnabled(logLevel))
                {
                    return;
                }

                // Buffer the message into a single string in order to avoid shearing the message when running across multiple threads.
                StringBuilder messageBuilder = new StringBuilder();

                string timestamp = this._logStart.HasValue ? $"{(DateTimeOffset.UtcNow - this._logStart.Value).TotalSeconds:N3}s" : DateTimeOffset.UtcNow.ToString(format: "s");

                string firstLinePrefix = $"| [{timestamp}] {this._category} {logLevel}: ";
                string[] lines = formatter(state, exception)
                    .Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
                string firstLine = lines.FirstOrDefault();
                messageBuilder.AppendLine(firstLinePrefix + firstLine);

                string additionalLinePrefix = "|" + new string(c: ' ', firstLinePrefix.Length - 1);

                foreach (string line in lines.Skip(count: 1))
                {
                    messageBuilder.AppendLine(additionalLinePrefix + line);
                }

                if (exception != null)
                {
                    lines = exception.ToString()
                        .Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
                    additionalLinePrefix = "| ";

                    foreach (string line in lines)
                    {
                        messageBuilder.AppendLine(additionalLinePrefix + line);
                    }
                }

                // Remove the last line-break, because ITestOutputHelper only has WriteLine.
                string message = messageBuilder.ToString();

                if (message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    message = message.Substring(startIndex: 0, message.Length - Environment.NewLine.Length);
                }

                try
                {
                    this._output.WriteLine(message);
                }
                catch (Exception)
                {
                    // We could fail because we're on a background thread and our captured ITestOutputHelper is
                    // busted (if the test "completed" before the background thread fired).
                    // So, ignore this. There isn't really anything we can do but hope the
                    // caller has additional loggers registered
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= this._minLogLevel;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new NullScope();
            }

            private sealed class NullScope : IDisposable
            {
                public void Dispose()
                {
                    // nothing to dispose
                }
            }
        }
    }
}