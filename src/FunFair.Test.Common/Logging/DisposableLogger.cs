using System;

namespace FunFair.Test.Common.Logging
{
    internal sealed class DisposableLogger : ILogger, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _scope;

        public DisposableLogger(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._scope = this._logger.BeginScope(state: "Test");
        }

        public void Dispose()
        {
            this._scope.Dispose();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this._logger.Log(logLevel: logLevel, eventId: eventId, state: state, exception: exception, formatter: formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this._logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this._logger.BeginScope(state);
        }
    }
}