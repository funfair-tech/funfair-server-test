using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Logging;

internal sealed class XUnitLogger : XUnitLoggerBase
{
    public XUnitLogger(
        ITestOutputHelper? testOutputHelper,
        LoggerExternalScopeProvider scopeProvider,
        string categoryName,
        in XUnitLoggerOptions options
    )
        : base(
            testOutputHelper: testOutputHelper,
            scopeProvider: scopeProvider,
            categoryName: categoryName,
            options: options
        ) { }
}
