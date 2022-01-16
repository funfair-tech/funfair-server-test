using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Helpers;

internal sealed class XunitLogger : ILogger
{
    private static readonly string[] NewLineChars =
    {
        Environment.NewLine
    };

    private readonly string _category;
    private readonly DateTimeOffset? _logStart;
    private readonly LogLevel _minLogLevel;
    private readonly ITestOutputHelper _output;

    public XunitLogger(ITestOutputHelper output, string category, LogLevel minLogLevel, DateTimeOffset? logStart)
    {
        this._minLogLevel = minLogLevel;
        this._category = category;
        this._output = output;
        this._logStart = logStart;
    }

    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0005:Avoid DateTimeOffset.UtcNow", Justification = "Unit test")]
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        // Buffer the message into a single string in order to avoid shearing the message when running across multiple threads.
        StringBuilder messageBuilder = new();

        string timestamp = this._logStart.HasValue
            ? $"{(DateTimeOffset.UtcNow - this._logStart.Value).TotalSeconds:N3}s"
            : DateTimeOffset.UtcNow.ToString(format: "s", formatProvider: CultureInfo.InvariantCulture);

        string firstLinePrefix = $"| [{timestamp}] {this._category} {logLevel}: ";
        string[] lines = formatter(arg1: state, arg2: exception)
            .Split(separator: NewLineChars, options: StringSplitOptions.RemoveEmptyEntries);
        string? firstLine = lines.FirstOrDefault();

        if (firstLine != null)
        {
            messageBuilder = messageBuilder.Append(firstLinePrefix)
                                           .AppendLine(firstLine);
        }

        string additionalLinePrefix = "|" + new string(c: ' ', firstLinePrefix.Length - 1);

        messageBuilder = lines.Skip(count: 1)
                              .Aggregate(seed: messageBuilder, func: (mb, line) => AppendMessageLine(mb: mb, line: line, additionalLinePrefix: additionalLinePrefix));

        if (HasException(exception))
        {
            lines = exception.ToString()
                             .Split(separator: NewLineChars, options: StringSplitOptions.RemoveEmptyEntries);
            additionalLinePrefix = "| ";

            messageBuilder = lines.Aggregate(seed: messageBuilder, func: (mb, line) => AppendMessageLine(mb: mb, line: line, additionalLinePrefix: additionalLinePrefix));
        }

        // Remove the last line-break, because ITestOutputHelper only has WriteLine.
        string message = messageBuilder.ToString();

        if (message.EndsWith(value: Environment.NewLine, comparisonType: StringComparison.Ordinal))
        {
            message = message.Substring(startIndex: 0, message.Length - Environment.NewLine.Length);
        }

        this.LogToOutput(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= this._minLogLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return new NullScope();
    }

    private static StringBuilder AppendMessageLine(StringBuilder mb, string line, string additionalLinePrefix)
    {
        return mb.Append(additionalLinePrefix)
                 .AppendLine(line);
    }

    private static bool HasException([NotNullWhen(true)] Exception? exception)
    {
        return exception != null;
    }

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Unit Test")]
    [SuppressMessage(category: "Roslynator.Analyzers", checkId: "RCS1075:DoNotCatchGeneralExceptionTypes", Justification = "Unit Test")]
    private void LogToOutput(string message)
    {
        try
        {
            this._output.WriteLine(message);
        }
        catch (Exception exception)
        {
            // We could fail because we're on a background thread and our captured ITestOutputHelper is
            // busted (if the test "completed" before the background thread fired).
            // So, ignore this. There isn't really anything we can do but hope the
            // caller has additional loggers registered
            Trace.WriteLine(exception.Message);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public void Dispose()
        {
            // nothing to dispose
        }
    }
}