using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Logging;

internal static class XUnitLoggingHelpers
{
    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "A mock of unit tests")]
    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0022:DisposeObjectsBeforeLosingScope", Justification = "A mock of unit tests")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used as part of test infrastructure")]
    public static ILoggingBuilder AddXUnit(this ILoggingBuilder builder, ITestOutputHelper output)
    {
        return builder.AddProvider(new XunitLoggerProvider(output));
    }
}