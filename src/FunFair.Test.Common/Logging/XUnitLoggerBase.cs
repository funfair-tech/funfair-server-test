using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

        StringBuilder sb = new();

        if (this._options.TimestampFormat is not null)
        {
            DateTimeOffset now = this.GetCurrentTimestamp();
            string timestamp = now.ToString(
                format: this._options.TimestampFormat,
                formatProvider: CultureInfo.InvariantCulture
            );
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

        sb = sb.Append(formatter(arg1: state, arg2: exception));

        if (exception is not null)
        {
            sb = sb.Append('\n').Append(exception);
        }

        // Append scopes
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

        try
        {
            testOutputHelper.WriteLine(sb.ToString());
        }
        catch (Exception ex)
        {
            // This can happen when the test is not active
            Debug.WriteLine(ex.Message);
        }
    }

    [SuppressMessage(
        category: "FunFair.CodeAnalysis",
        checkId: "FFS0004: Use IDateTimeSource.UtcNow()",
        Justification = "Not available here"
    )]
    [SuppressMessage(
        category: "FunFair.CodeAnalysis",
        checkId: "FFS0005: Use IDateTimeSource.UtcNow()",
        Justification = "Not available here"
    )]
    private DateTimeOffset GetCurrentTimestamp()
    {
        return this._options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
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
