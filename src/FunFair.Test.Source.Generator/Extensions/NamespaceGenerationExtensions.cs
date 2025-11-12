using FunFair.Test.Source.Generator.Builders;
using FunFair.Test.Source.Generator.Constants;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator.Extensions;

internal static class NamespaceGenerationExtensions
{
    private static string GetNamespace(in this NamespaceGeneration namespaceGeneration, in AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
    {
        return GetRootNameSpace(analyzerConfigOptionsProvider: analyzerConfigOptionsProvider) ?? namespaceGeneration.Namespace;
    }

    private static string? GetRootNameSpace(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
    {
        return analyzerConfigOptionsProvider.GlobalOptions.TryGetValue(key: "build_property.rootnamespace", out string? ns) && !string.IsNullOrWhiteSpace(ns)
            ? ns
            : null;
    }

    public static CodeBuilder BuildSource(in this NamespaceGeneration namespaceGeneration, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, out string ns)
    {
        ns = namespaceGeneration.GetNamespace(analyzerConfigOptionsProvider: analyzerConfigOptionsProvider);

        CodeBuilder source = new();

        using (source.AppendFileHeader()
                     .AppendNamespaces("System",
                                       "System.CodeDom.Compiler",
                                       "System.Diagnostics.CodeAnalysis",
                                       "System.Linq",
                                       "System.Threading.Tasks",
                                       "Xunit.MicrosoftTestingPlatform",
                                       "Xunit.Runner.InProc.SystemConsole")
                     .AppendBlankLine()
                     .AppendLine($"namespace {ns};")
                     .AppendBlankLine()
                     .AppendGeneratedCodeAttribute()
                     .StartBlock($"internal static class {AppConstants.ClassName}"))
        {
            source = source.AppendLine(
                "");

            using (source.StartBlock("public static Task<int> Main(string[] args)"))
            {
                source = source.AppendLine("return args.Any(predicate: ConsoleRunnerDetected)")
                               .AppendLine("    ? ConsoleRunner.Run(args)")
                               .AppendLine("    : TestPlatformTestFramework.RunAsync(args: args, extensionRegistration: SelfRegisteredExtensions.AddSelfRegisteredExtensions);");
            }

            using (source.StartBlock("private static bool ConsoleRunnerDetected(string arg)"))
            {
                source = source.AppendLine("return StringComparer.OrdinalIgnoreCase.Equals(x: arg, y: \"-automated\") || StringComparer.Ordinal.Equals(x: arg, y: \"@@\");");
            }
        }

        return source;
    }
}