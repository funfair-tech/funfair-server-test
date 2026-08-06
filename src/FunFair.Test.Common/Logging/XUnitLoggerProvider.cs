using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Logging;

internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly XUnitLoggerOptions _options;
    private readonly LoggerExternalScopeProvider _scopeProvider = new();
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        : this(testOutputHelper: testOutputHelper, options: null) { }

    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper, XUnitLoggerOptions? options)
    {
        this._testOutputHelper = testOutputHelper;
        this._options = options ?? XUnitLoggerOptions.Default;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(
            testOutputHelper: this._testOutputHelper,
            scopeProvider: this._scopeProvider,
            categoryName: categoryName,
            options: this._options
        );
    }

    public void Dispose() { }
}
