using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

internal static class GeneratorTestHelpers
{
    private static readonly Lazy<IReadOnlyList<MetadataReference>> References = new(BuildReferences);

    private static Compilation CreateCompilation(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            text: source,
            cancellationToken: TestContext.Current.CancellationToken
        );

        return CSharpCompilation.Create(
            assemblyName: "GeneratorTestAssembly",
            syntaxTrees: [syntaxTree],
            references: References.Value,
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

    public static GeneratorDriverRunResult RunGenerator(IIncrementalGenerator generator, string source)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: null,
            parseOptions: null,
            optionsProvider: null
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
        Compilation compilation = CreateCompilation(source);

        Assert.Empty(compilation.GetDiagnostics(TestContext.Current.CancellationToken).Where(IsError));

        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers([analyzer]);

        return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }

    private static bool IsError(Diagnostic diagnostic)
    {
        return diagnostic.Severity == DiagnosticSeverity.Error;
    }
}
