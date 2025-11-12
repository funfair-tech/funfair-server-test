using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Logging;

internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly XUnitLoggerOptions _options;
    private readonly LoggerExternalScopeProvider _scopeProvider = new();
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>Initializes a new instance of the <see cref="XUnitLoggerProvider" /> class with the specified test output helper.</summary>
    /// <param name="testOutputHelper">The xUnit.net test output helper.</param>
    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        : this(testOutputHelper: testOutputHelper, options: null) { }

    /// <summary>Initializes a new instance of the <see cref="XUnitLoggerProvider" /> class with the specified test output helper and options.</summary>
    /// <param name="testOutputHelper">The xUnit.net test output helper.</param>
    /// <param name="options">The logger options.</param>
    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper, XUnitLoggerOptions? options)
    {
        this._testOutputHelper = testOutputHelper;
        this._options = options ?? new XUnitLoggerOptions();
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(
            testOutputHelper: this._testOutputHelper,
            scopeProvider: this._scopeProvider,
            categoryName: categoryName,
            options: this._options
        );
    }

    /// <inheritdoc />
    public void Dispose() { }
}
