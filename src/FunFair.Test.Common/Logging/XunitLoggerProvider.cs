using System;
using System.Threading;
using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Logging;

internal sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly DateTimeOffset? _logStart;
    private readonly LogLevel _minLevel;
    private readonly ITestOutputHelper _output;

    private readonly Semaphore _semaphore;

    public XunitLoggerProvider(ITestOutputHelper output)
        : this(output: output, minLevel: LogLevel.Trace)
    {
    }

    public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel)
        : this(output: output, minLevel: minLevel, logStart: null)
    {
    }

    public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel, DateTimeOffset? logStart)
    {
        this._output = output;
        this._minLevel = minLevel;
        this._logStart = logStart;

        this._semaphore = new(initialCount: 1, maximumCount: 1, name: "TeamCity");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(output: this._output, category: categoryName, minLogLevel: this._minLevel, logStart: this._logStart, semaphore: this._semaphore);
    }

    public void Dispose()
    {
        // nothing to dispose
        this._semaphore.Dispose();
    }
}