using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Infrastructure.Mocks;

public sealed class CapturingLogger<T> : ILogger<T>
{
    private readonly List<CapturedLogEntry> _entries = [];
    private readonly Lock _lock = new();

    public IReadOnlyList<CapturedLogEntry> Entries
    {
        get
        {
            lock (this._lock)
            {
                return [.. this._entries];
            }
        }
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        string message = formatter(arg1: state, arg2: exception);

        lock (this._lock)
        {
            this._entries.Add(new(Level: logLevel, EventId: eventId, Message: message));
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }
}
