using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Logging;

internal abstract class XUnitLoggerBase : ILogger
{
    private readonly string? _categoryText;
    private readonly XUnitLoggerOptions _options;
    private readonly LoggerExternalScopeProvider _scopeProvider;
    private readonly ITestOutputHelper? _testOutputHelper;

    protected XUnitLoggerBase(
        ITestOutputHelper? testOutputHelper,
        LoggerExternalScopeProvider scopeProvider,
        string? categoryName,
        in XUnitLoggerOptions options
    )
    {
        this._testOutputHelper = testOutputHelper;
        this._scopeProvider = scopeProvider;
        this._categoryText = options.IncludeCategory ? $"[{categoryName}] " : null;
        this._options = options;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel is not LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return this._scopeProvider.Push(state);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        ITestOutputHelper? testOutputHelper = this._testOutputHelper ?? TestContext.Current.TestOutputHelper;

        if (testOutputHelper is null)
        {
            return;
        }

        string message = formatter(arg1: state, arg2: exception);

        if (!this.NeedsFormatting(exception))
        {
            WriteLine(testOutputHelper: testOutputHelper, message: message);

            return;
        }

        string formatted = this.BuildFormattedMessage(logLevel: logLevel, message: message, exception: exception);

        WriteLine(testOutputHelper: testOutputHelper, message: formatted);
    }

    private bool NeedsFormatting(Exception? exception)
    {
        return exception is not null || this._options.RequiresFormatting;
    }

    private string BuildFormattedMessage(LogLevel logLevel, string message, Exception? exception)
    {
        string? timestamp = this._options.TimestampFormat is null
            ? null
            : this.GetCurrentTimestamp()
                .ToString(format: this._options.TimestampFormat, formatProvider: CultureInfo.InvariantCulture);
        string? exceptionText = exception?.ToString();
        string? logLevelText = this._options.IncludeLogLevel ? GetLogLevelString(logLevel) : null;

        int capacity =
            message.Length
            + LengthWithSeparator(timestamp, separatorLength: 1)
            + LengthWithSeparator(logLevelText, separatorLength: 1)
            + LengthWithSeparator(this._categoryText)
            + LengthWithSeparator(exceptionText, separatorLength: 1);

        StringBuilder sb = new(capacity: capacity);

        if (timestamp is not null)
        {
            sb.Append(timestamp).Append(' ');
        }

        if (logLevelText is not null)
        {
            sb.Append(logLevelText).Append(' ');
        }

        sb.Append(this._categoryText).Append(message);

        if (exceptionText is not null)
        {
            sb.Append('\n').Append(exceptionText);
        }

        if (this._options.IncludeScopes)
        {
            this._scopeProvider.ForEachScope(
                callback: (scope, state) =>
                {
                    state.Append("\n => ");
                    state.Append(scope);
                },
                state: sb
            );
        }

        return sb.ToString();
    }

    private static void WriteLine(ITestOutputHelper testOutputHelper, string message)
    {
        try
        {
            testOutputHelper.WriteLine(message);
        }
        catch (Exception ex)
        {
            // This can happen when the test is not active
            Debug.WriteLine(ex.Message);
        }
    }

    private DateTimeOffset GetCurrentTimestamp()
    {
        return this._options.GetCurrentTimestamp(TimeProvider.System);
    }

    private static int LengthWithSeparator(string? value, int separatorLength = 0)
    {
        return value is null ? 0 : value.Length + separatorLength;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }
}
