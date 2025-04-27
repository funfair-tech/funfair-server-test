using System.Collections.Generic;
using FunFair.Test.Common.Extensions;
using FunFair.Test.Common.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Tests.Mocks;

public sealed class MockLoggerTests : TestBase
{
    private readonly ILogger _baseLogger;
    private readonly MockLogger<ExampleObject> _logger;

    public MockLoggerTests()
    {
        this._baseLogger = GetSubstitute<ILogger>();
        this._logger = new(this._baseLogger);
    }

    [Fact]
    public void LogCritical()
    {
        this._logger.LogCritical("Critical Error");

        Assert.True(condition: this._logger.CriticalReported, userMessage: "Critical should have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(
            condition: this._logger.InformationReported,
            userMessage: "Information should not have been reported"
        );
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Critical] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.Received(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    [Fact]
    public void LogError()
    {
        this._logger.LogError("Error");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.True(condition: this._logger.ErrorsReported, userMessage: "Errors should have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(
            condition: this._logger.InformationReported,
            userMessage: "Information should not have been reported"
        );
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Error] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.Received(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    [Fact]
    public void LogWarning()
    {
        this._logger.LogWarning("Warning");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.True(condition: this._logger.WarningsReported, userMessage: "Warnings should have been reported");
        Assert.False(
            condition: this._logger.InformationReported,
            userMessage: "Information should not have been reported"
        );
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Warning] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.Received(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    [Fact]
    public void LogInformation()
    {
        this._logger.LogInformation("Information");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.True(condition: this._logger.InformationReported, userMessage: "Information should have been reported");
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Information] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.Received(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    [Fact]
    public void LogDebug()
    {
        this._logger.LogDebug("Debug");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(condition: this._logger.InformationReported, userMessage: "Information should have been reported");
        Assert.True(condition: this._logger.DebugReported, userMessage: "Debug should have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Debug] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.Received(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    [Fact]
    public void LogTrace()
    {
        this._logger.LogTrace("Trace");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(condition: this._logger.InformationReported, userMessage: "Information should have been reported");
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should have been reported");
        Assert.True(condition: this._logger.TraceReported, userMessage: "Trace should have been reported");

        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [LogLevel.Trace] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);

        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
        this._baseLogger.Received(logLevel: LogLevel.Trace, message: "Trace");
    }
}
