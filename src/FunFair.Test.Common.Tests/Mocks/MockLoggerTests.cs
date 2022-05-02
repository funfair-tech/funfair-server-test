using FunFair.Test.Common.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Tests.Mocks;

public sealed class MockLoggerTests : TestBase
{
    private readonly MockLogger<ExampleObject> _logger;

    public MockLoggerTests()
    {
        this._logger = new(GetSubstitute<ILogger>());
    }

    [Fact]
    public void LogCritical()
    {
        this._logger.LogCritical("Critical Error");

        Assert.True(condition: this._logger.CriticalReported, userMessage: "Critical should have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(condition: this._logger.InformationReported, userMessage: "Information should not have been reported");
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");
    }

    [Fact]
    public void LogError()
    {
        this._logger.LogError("Error");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.True(condition: this._logger.ErrorsReported, userMessage: "Errors should have been reported");
        Assert.False(condition: this._logger.WarningsReported, userMessage: "Warnings should not have been reported");
        Assert.False(condition: this._logger.InformationReported, userMessage: "Information should not have been reported");
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");
    }

    [Fact]
    public void LogWarning()
    {
        this._logger.LogWarning("Warning");

        Assert.False(condition: this._logger.CriticalReported, userMessage: "Critical should not have been reported");
        Assert.False(condition: this._logger.ErrorsReported, userMessage: "Errors should not have been reported");
        Assert.True(condition: this._logger.WarningsReported, userMessage: "Warnings should have been reported");
        Assert.False(condition: this._logger.InformationReported, userMessage: "Information should not have been reported");
        Assert.False(condition: this._logger.DebugReported, userMessage: "Debug should not have been reported");
        Assert.False(condition: this._logger.TraceReported, userMessage: "Trace should not have been reported");
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
    }
}