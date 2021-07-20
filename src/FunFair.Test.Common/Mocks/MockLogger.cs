using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Mock of logging.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public sealed class MockLogger<T> : ILogger<T>
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<LogLevel, LogCounter> _seen;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public MockLogger([SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024: Logger parameters should be ILogger<T>", Justification = "Not created through DI")]
                          ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._seen = new ConcurrentDictionary<LogLevel, LogCounter>();
        }

        /// <summary>
        ///     Summary of all the items that have been seen.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public IReadOnlyDictionary<LogLevel, int> Seen => this._seen.ToDictionary(keySelector: k => k.Key, elementSelector: v => v.Value.Count);

        /// <summary>
        ///     Have Critical errors been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool CriticalReported => this._seen.ContainsKey(LogLevel.Critical);

        /// <summary>
        ///     Have errors been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool ErrorsReported => this._seen.ContainsKey(LogLevel.Error);

        /// <summary>
        ///     Have warnings been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool WarningsReported => this._seen.ContainsKey(LogLevel.Warning);

        /// <summary>
        ///     Have trace messages been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool TraceReported => this._seen.ContainsKey(LogLevel.Trace);

        /// <summary>
        ///     Has Information been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool InformationReported => this._seen.ContainsKey(LogLevel.Information);

        /// <summary>
        ///     Have debug messages been reported.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public bool DebugReported => this._seen.ContainsKey(LogLevel.Debug);

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (state != null)
            {
                this._logger.Log<object>(logLevel: logLevel, eventId: eventId, state: state, exception: exception, formatter: (_, _) => string.Empty);
            }

            LogCounter counter = this._seen.GetOrAdd(key: logLevel, new LogCounter());

            counter.Increment();
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return this._logger.IsEnabled(logLevel);
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return this._logger.BeginScope<object>(state);
        }

        private sealed class LogCounter
        {
            private long _count;

            public LogCounter()
            {
                this._count = 0;
            }

            public int Count => (int) Interlocked.Read(ref this._count);

            public void Increment()
            {
                Interlocked.Increment(ref this._count);
            }
        }
    }
}