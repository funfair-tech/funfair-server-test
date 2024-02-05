using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace FunFair.Test.Common.Mocks;

[DebuggerDisplay("Critical: {CriticalReported} Errors: {ErrorsReported} Warnings: {WarningsReported} Trace: {TraceReported} Information: {InformationReported} Debug: {DebugReported}")]
public sealed class MockLogger<T> : ILogger<T>
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<LogLevel, LogCounter> _seen;

    public MockLogger([SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024: Logger parameters should be ILogger<T>", Justification = "Not created through DI")] ILogger logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._seen = new();
    }

    public IReadOnlyDictionary<LogLevel, int> Seen => this._seen.ToDictionary(keySelector: k => k.Key, elementSelector: v => v.Value.Count);

    public bool CriticalReported => this._seen.ContainsKey(LogLevel.Critical);

    public bool ErrorsReported => this._seen.ContainsKey(LogLevel.Error);

    public bool WarningsReported => this._seen.ContainsKey(LogLevel.Warning);

    public bool TraceReported => this._seen.ContainsKey(LogLevel.Trace);

    public bool InformationReported => this._seen.ContainsKey(LogLevel.Information);

    public bool DebugReported => this._seen.ContainsKey(LogLevel.Debug);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (HasValidState(state))
        {
            this._logger.Log<object>(logLevel: logLevel, eventId: eventId, state: state, exception: exception, formatter: (_, _) => string.Empty);
        }

        LogCounter counter = this.GetLogCounter(logLevel);

        counter.Increment();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return this._logger.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return this._logger.BeginScope<object>(state) ?? ThrowInvalidOperationException();
    }

    private LogCounter GetLogCounter(LogLevel logLevel)
    {
        return this._seen.TryGetValue(key: logLevel, out LogCounter? counter)
            ? counter
            : this._seen.GetOrAdd(key: logLevel, new LogCounter());
    }

    [DoesNotReturn]
    private static IDisposable ThrowInvalidOperationException()
    {
        throw new InvalidOperationException();
    }

    private static bool HasValidState([NotNullWhen(true)] object? state)
    {
        return state is not null;
    }

    [DebuggerDisplay("Count: {Count}")]
    private sealed class LogCounter
    {
        private long _count;

        public LogCounter()
        {
            this._count = 0;
        }

        public int Count => (int)Interlocked.Read(ref this._count);

        public void Increment()
        {
            Interlocked.Increment(ref this._count);
        }
    }
}