using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Mock of logging.
    /// </summary>
    /// <typeparam name="T">The logging type.</typeparam>
    public sealed class MockLogger<T> : ILogger<T>
    {
        private readonly ConcurrentDictionary<LogLevel, int> _seen;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public MockLogger()
        {
            this._seen = new ConcurrentDictionary<LogLevel, int>();
        }

        /// <summary>
        ///     Summary of all the items that have been seen.
        /// </summary>
        public IReadOnlyDictionary<LogLevel, int> Seen => this._seen;

        /// <summary>
        ///     Have Critical errors been reported.
        /// </summary>
        public bool CriticalReported => this._seen.ContainsKey(LogLevel.Critical);

        /// <summary>
        ///     Have errors been reported.
        /// </summary>
        public bool ErrorsReported => this._seen.ContainsKey(LogLevel.Error);

        /// <summary>
        ///     Have warnings been reported.
        /// </summary>
        public bool WarningsReported => this._seen.ContainsKey(LogLevel.Warning);

        /// <summary>
        ///     Have trace messages been reported.
        /// </summary>
        public bool TraceReported => this._seen.ContainsKey(LogLevel.Trace);

        /// <summary>
        ///     Has Information been reported.
        /// </summary>
        public bool InformationReported => this._seen.ContainsKey(LogLevel.Information);

        /// <summary>
        ///     Have debug messages been reported.
        /// </summary>
        public bool DebugReported => this._seen.ContainsKey(LogLevel.Debug);

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this._seen.AddOrUpdate(logLevel, addValueFactory: lvl => 1, updateValueFactory: (lvl, count) => count + 1);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}