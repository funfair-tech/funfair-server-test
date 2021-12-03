using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Logging;

internal sealed class LogOutput : ITestOutputHelper
{
    private readonly ILogger _logger;

    public LogOutput(
        [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024: Logger parameters should be ILogger<T>", Justification = "Not created through DI")] ILogger logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void WriteLine(string message)
    {
        this._logger.LogDebug(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        this._logger.LogDebug(message: format, args: args);
    }
}