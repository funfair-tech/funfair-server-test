using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

internal static class GeneratorTestHelpers
{
    private static readonly Lazy<IReadOnlyList<MetadataReference>> References = new(BuildReferences);

    public static Compilation CreateCompilation(string source)
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

    public static GeneratorDriverRunResult RunGenerator(
        IIncrementalGenerator generator,
        Compilation compilation,
        ImmutableDictionary<string, string>? globalOptions = null
    )
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: null,
            parseOptions: null,
            optionsProvider: new TestAnalyzerConfigOptionsProvider(globalOptions ?? [])
        );

        driver = driver.RunGenerators(compilation, cancellationToken: TestContext.Current.CancellationToken);

        return driver.GetRunResult();
    }

    public static GeneratorDriverRunResult RunGenerator(
        IIncrementalGenerator generator,
        string source,
        ImmutableDictionary<string, string>? globalOptions = null
    )
    {
        GeneratorDriverRunResult result = RunGenerator(
            generator: generator,
            compilation: CreateCompilation(source),
            globalOptions: globalOptions
        );

        Assert.Empty(result.Diagnostics);

        return result;
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        public TestAnalyzerConfigOptionsProvider(ImmutableDictionary<string, string> globalOptions)
        {
            this.GlobalOptions = new TestAnalyzerConfigOptions(globalOptions);
        }

        public override AnalyzerConfigOptions GlobalOptions { get; }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return this.GlobalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return this.GlobalOptions;
        }
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly ImmutableDictionary<string, string> _options;

        public TestAnalyzerConfigOptions(ImmutableDictionary<string, string> options)
        {
            this._options = options;
        }

        public override IEnumerable<string> Keys => this._options.Keys;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            return this._options.TryGetValue(key: key, value: out value!);
        }
    }
}
