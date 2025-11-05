using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit.MicrosoftTestingPlatform;
using Xunit.Runner.InProc.SystemConsole;

namespace FunFair.Test.Common.Tests;

[ExcludeFromCodeCoverage]
public static class TestEntryPoint
{
    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0109: Add an overload with a Span or Memory parameter", Justification = "Won't work here")]
    public static Task<int> Main(string[] args)
    {
        return args.Any(predicate: ConsoleRunnerDetected)
            ? ConsoleRunner.Run(args)
            : TestPlatformTestFramework.RunAsync(args: args, extensionRegistration: SelfRegisteredExtensions.AddSelfRegisteredExtensions);
    }

    private static bool ConsoleRunnerDetected(string arg)
    {
        return StringComparer.OrdinalIgnoreCase.Equals(x: arg, y: "-automated") || StringComparer.Ordinal.Equals(x: arg, y: "@@");
    }
}