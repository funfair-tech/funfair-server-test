using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator.Extensions;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    // Set by a project that wants this assembly's analyzers but not its test-assembly generators: FunFair.Test.Common
    // is a shipped library, not a test assembly, so it must not gain the generated
    // [assembly: ExcludeFromCodeCoverage]/SuppressMessage attributes. The package's build/ props make it
    // compiler-visible, so a consumer that leaves it unset sees it as an empty value, which means generate.
    private const string DISABLE_SOURCE_GENERATORS_PROPERTY = "build_property.FunFairTestDisableSourceGenerators";

    public static bool AreSourceGeneratorsDisabled(this AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
    {
        return analyzerConfigOptionsProvider.GlobalOptions.TryGetValue(
                key: DISABLE_SOURCE_GENERATORS_PROPERTY,
                out string? value
            ) && StringComparer.OrdinalIgnoreCase.Equals(x: value, y: "true");
    }
}
