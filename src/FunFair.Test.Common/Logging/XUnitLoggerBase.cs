using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Logging;

internal abstract class XUnitLoggerBase : ILogger
{
    private readonly string? _categoryName;
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
        this._categoryName = categoryName;
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

        if (exception is null && !this._options.RequiresFormatting)
        {
            WriteLine(testOutputHelper: testOutputHelper, message: message);

            return;
        }

        string formatted = this.BuildFormattedMessage(logLevel: logLevel, message: message, exception: exception);

        WriteLine(testOutputHelper: testOutputHelper, message: formatted);
    }

    private string BuildFormattedMessage(LogLevel logLevel, string message, Exception? exception)
    {
        string? timestamp = this._options.TimestampFormat is null
            ? null
            : this.GetCurrentTimestamp()
                .ToString(format: this._options.TimestampFormat, formatProvider: CultureInfo.InvariantCulture);
        string? exceptionText = exception?.ToString();

        int capacity =
            message.Length
            + (timestamp is null ? 0 : timestamp.Length + 1)
            + (this._options.IncludeLogLevel ? 5 : 0)
            + (this._options.IncludeCategory ? (this._categoryName?.Length ?? 0) + 3 : 0)
            + (exceptionText is null ? 0 : exceptionText.Length + 1);

        StringBuilder sb = new(capacity: capacity);

        if (timestamp is not null)
        {
            sb = sb.Append(timestamp).Append(' ');
        }

        if (this._options.IncludeLogLevel)
        {
            sb = sb.Append(GetLogLevelString(logLevel)).Append(' ');
        }

        if (this._options.IncludeCategory)
        {
            sb = sb.Append('[').Append(this._categoryName).Append("] ");
        }

        sb = sb.Append(message);

        if (exceptionText is not null)
        {
            sb = sb.Append('\n').Append(exceptionText);
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
