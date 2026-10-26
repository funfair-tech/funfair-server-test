using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Test.Source.Generator.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

internal static class GeneratorTestHelpers
{
    private static readonly Lazy<IReadOnlyList<MetadataReference>> References = new(BuildReferences);

    private static readonly Lazy<IReadOnlyList<MetadataReference>> ReferencesWithoutXunit = new(
        BuildReferencesWithoutXunit
    );

    private static readonly Lazy<IReadOnlyList<MetadataReference>> ReferencesWithoutFunFairTestCommon = new(
        BuildReferencesWithoutFunFairTestCommon
    );

    private static Compilation CreateCompilation(string source)
    {
        return CreateCompilation(source: source, references: References.Value);
    }

    private static Compilation CreateCompilation(string source, IReadOnlyList<MetadataReference> references)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            text: source,
            cancellationToken: TestContext.Current.CancellationToken
        );

        return CSharpCompilation.Create(
            assemblyName: "GeneratorTestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        return
        [
            .. AppDomain
                .CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                .Select(assembly => (MetadataReference)MetadataReference.CreateFromFile(assembly.Location)),
        ];
    }

    private static IReadOnlyList<MetadataReference> BuildReferencesWithoutXunit()
    {
        return
        [
            .. References.Value.Where(reference =>
                reference.Display?.Contains("xunit", StringComparison.OrdinalIgnoreCase) != true
            ),
        ];
    }

    private static IReadOnlyList<MetadataReference> BuildReferencesWithoutFunFairTestCommon()
    {
        return
        [
            .. References.Value.Where(reference =>
                !StringComparer.OrdinalIgnoreCase.Equals(
                    x: Path.GetFileNameWithoutExtension(reference.Display),
                    y: "FunFair.Test.Common"
                )
            ),
        ];
    }

    public static GeneratorDriverRunResult RunGenerator(IIncrementalGenerator generator, string source)
    {
        return RunGenerator(generator: generator, source: source, optionsProvider: null);
    }

    public static GeneratorDriverRunResult RunGenerator(
        IIncrementalGenerator generator,
        string source,
        IReadOnlyDictionary<string, string> globalOptions
    )
    {
        return RunGenerator(
            generator: generator,
            source: source,
            optionsProvider: new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions(globalOptions))
        );
    }

    // Records each named pipeline step, so a test can prove whether a step ran at all, not just what it emitted.
    public static GeneratorDriverRunResult RunGeneratorTrackingSteps(
        IIncrementalGenerator generator,
        string source,
        IReadOnlyDictionary<string, string> globalOptions
    )
    {
        return RunGenerator(
            generator: generator,
            source: source,
            optionsProvider: new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions(globalOptions)),
            driverOptions: new GeneratorDriverOptions(
                disabledOutputs: IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true
            )
        );
    }

    private static GeneratorDriverRunResult RunGenerator(
        IIncrementalGenerator generator,
        string source,
        AnalyzerConfigOptionsProvider? optionsProvider
    )
    {
        return RunGenerator(
            generator: generator,
            source: source,
            optionsProvider: optionsProvider,
            driverOptions: default
        );
    }

    private static GeneratorDriverRunResult RunGenerator(
        IIncrementalGenerator generator,
        string source,
        AnalyzerConfigOptionsProvider? optionsProvider,
        in GeneratorDriverOptions driverOptions
    )
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: null,
            parseOptions: null,
            optionsProvider: optionsProvider,
            driverOptions: driverOptions
        );

        driver = driver.RunGenerators(
            CreateCompilation(source),
            cancellationToken: TestContext.Current.CancellationToken
        );

        GeneratorDriverRunResult result = driver.GetRunResult();

        Assert.Empty(result.Diagnostics);

        return result;
    }

    public static Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync(DiagnosticAnalyzer analyzer, string source)
    {
        return RunAnalyzerAsync(analyzer: analyzer, source: source, references: References.Value);
    }

    // Simulates the analyzer running on a project with no xunit reference at all (e.g. a non-test
    // project that only picks up this analyzer package transitively) - AotTestDispatcherAnalyzer must
    // no-op rather than crash when xunit's types aren't resolvable.
    public static Task<ImmutableArray<Diagnostic>> RunAnalyzerWithoutXunitReferenceAsync(
        DiagnosticAnalyzer analyzer,
        string source
    )
    {
        return RunAnalyzerAsync(analyzer: analyzer, source: source, references: ReferencesWithoutXunit.Value);
    }

    // Simulates the analyzer running on a project that does not reference FunFair.Test.Common, so
    // FunFair.Test.Common.DispatchedTestAttribute only resolves if the source under test declares it.
    public static Task<ImmutableArray<Diagnostic>> RunAnalyzerWithoutFunFairTestCommonReferenceAsync(
        DiagnosticAnalyzer analyzer,
        string source
    )
    {
        return RunAnalyzerAsync(
            analyzer: analyzer,
            source: source,
            references: ReferencesWithoutFunFairTestCommon.Value
        );
    }

    private static Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync(
        DiagnosticAnalyzer analyzer,
        string source,
        IReadOnlyList<MetadataReference> references
    )
    {
        Compilation compilation = CreateCompilation(source: source, references: references);

        Assert.Empty(compilation.GetDiagnostics(TestContext.Current.CancellationToken).Where(IsError));

        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers([analyzer]);

        return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }

    private static bool IsError(Diagnostic diagnostic)
    {
        return diagnostic.Severity == DiagnosticSeverity.Error;
    }
}
