using System;
using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Logging
{
    internal sealed class XunitLoggerProvider : ILoggerProvider
    {
        private readonly DateTimeOffset? _logStart;
        private readonly LogLevel _minLevel;
        private readonly ITestOutputHelper _output;

        public XunitLoggerProvider(ITestOutputHelper output)
            : this(output, LogLevel.Trace)
        {
        }

        public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel)
            : this(output, minLevel, logStart: null)
        {
        }

        public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel, DateTimeOffset? logStart)
        {
            this._output = output;
            this._minLevel = minLevel;
            this._logStart = logStart;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitLogger(this._output, categoryName, this._minLevel, this._logStart);
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}