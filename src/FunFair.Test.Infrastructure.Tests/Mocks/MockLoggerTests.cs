using System.Collections.Generic;
using FunFair.Test.Common;
using FunFair.Test.Infrastructure.Extensions;
using FunFair.Test.Infrastructure.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Infrastructure.Tests.Mocks;

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

        this.AssertCriticalWasReported();
        this.AssertErrorsWereNotReported();
        this.AssertWarningsWereNotReported();
        this.AssertInformationWasNotReported();
        this.AssertDebugWasNotReported();
        this.AssertTraceWasNotReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Critical);
    }

    [Fact]
    public void LogError()
    {
        this._logger.LogError("Error");

        this.AssertCriticalWasNotReported();
        this.AssertErrorsWereReported();
        this.AssertWarningsWereNotReported();
        this.AssertInformationWasNotReported();
        this.AssertDebugWasNotReported();
        this.AssertTraceWasNotReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Error);
    }

    [Fact]
    public void LogWarning()
    {
        this._logger.LogWarning("Warning");

        this.AssertCriticalWasNotReported();
        this.AssertErrorsWereNotReported();
        this.AssertWarningsWereReported();
        this.AssertInformationWasNotReported();
        this.AssertDebugWasNotReported();
        this.AssertTraceWasNotReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Warning);
    }

    [Fact]
    public void LogInformation()
    {
        this._logger.LogInformation("Information");

        this.AssertCriticalWasNotReported();
        this.AssertErrorsWereNotReported();
        this.AssertWarningsWereNotReported();
        this.AssertInformationWasReported();
        this.AssertDebugWasNotReported();
        this.AssertTraceWasNotReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Information);
    }

    [Fact]
    public void LogDebug()
    {
        this._logger.LogDebug("Debug");

        this.AssertCriticalWasNotReported();
        this.AssertErrorsWereNotReported();
        this.AssertWarningsWereNotReported();
        this.AssertInformationWasNotReported();
        this.AssertDebugWasReported();
        this.AssertTraceWasNotReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Debug);
    }

    [Fact]
    public void LogTrace()
    {
        this._logger.LogTrace("Trace");

        this.AssertCriticalWasNotReported();
        this.AssertErrorsWereNotReported();
        this.AssertWarningsWereNotReported();
        this.AssertInformationWasNotReported();
        this.AssertDebugWasNotReported();
        this.AssertTraceWasReported();
        this.AssertOnlyLevelWasSeen(LogLevel.Trace);
    }

    private void AssertCriticalWasReported()
    {
        Assert.True(condition: this._logger.CriticalReported, userMessage: "Critical should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Critical, message: "Critical Error");
    }

    private void AssertCriticalWasNotReported()
    {
        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Critical, message: "Critical Error");
    }

    private void AssertErrorsWereReported()
    {
        Assert.True(condition: this._logger.ErrorsReported, userMessage: "Errors should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Error, message: "Error");
    }

    private void AssertErrorsWereNotReported()
    {
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Error, message: "Error");
    }

    private void AssertWarningsWereReported()
    {
        Assert.True(condition: this._logger.WarningsReported, userMessage: "Warnings should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Warning, message: "Warning");
    }

    private void AssertWarningsWereNotReported()
    {
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Warning, message: "Warning");
    }

    private void AssertInformationWasReported()
    {
        Assert.True(condition: this._logger.InformationReported, userMessage: "Information should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Information, message: "Information");
    }

    private void AssertInformationWasNotReported()
    {
        Assert.False(
            condition: this._logger.InformationReported,
            userMessage: "Information should not have been reported"
        );
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Information, message: "Information");
    }

    private void AssertDebugWasReported()
    {
        Assert.True(condition: this._logger.DebugReported, userMessage: "Debug should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Debug, message: "Debug");
    }

    private void AssertDebugWasNotReported()
    {
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Debug, message: "Debug");
    }

    private void AssertTraceWasReported()
    {
        Assert.True(condition: this._logger.TraceReported, userMessage: "Trace should have been reported");
        this._baseLogger.Received(logLevel: LogLevel.Trace, message: "Trace");
    }

    private void AssertTraceWasNotReported()
    {
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");
        this._baseLogger.DidNotReceive(logLevel: LogLevel.Trace, message: "Trace");
    }

    private void AssertOnlyLevelWasSeen(LogLevel level)
    {
        IReadOnlyDictionary<LogLevel, int> items = new Dictionary<LogLevel, int> { [level] = 1 };
        Assert.Equal(expected: items, actual: this._logger.Seen);
    }
}
