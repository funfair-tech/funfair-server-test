using System;
using System.Linq;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

public sealed class TestAssemblyCodeGeneratorTests : TestBase
{
    private const string AssemblySettingsHintName = "AssemblySettings.generated.cs";

    [Fact]
    public void SingleNamespace_GeneratesAssemblySettings()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new TestAssemblyCodeGenerator(),
            source: """
            namespace Sample;

            public sealed class Example { }
            """
        );

        GeneratedSourceResult generated = Assert.Single(
            result.Results.Single().GeneratedSources,
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: AssemblySettingsHintName)
        );

        string text = generated.SourceText.ToString();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", text, StringComparison.Ordinal);
        Assert.Contains("[assembly: ExcludeFromCodeCoverage]", text, StringComparison.Ordinal);
        Assert.Contains("PH2140", text, StringComparison.Ordinal);
        Assert.Contains("PH2071", text, StringComparison.Ordinal);
        Assert.Contains("PH2088", text, StringComparison.Ordinal);
        Assert.Contains("CA1873", text, StringComparison.Ordinal);
        Assert.Contains("CA2254", text, StringComparison.Ordinal);
    }

    [Fact]
    public void MultipleNamespacesInSameAssembly_GeneratesAssemblySettingsOnlyOnce()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new TestAssemblyCodeGenerator(),
            source: """
            namespace Sample.First
            {
                public sealed class ExampleOne { }
            }

            namespace Sample.Second
            {
                public sealed class ExampleTwo { }
            }

            namespace Sample.Third
            {
                public sealed class ExampleThree { }
            }
            """
        );

        int assemblySettingsCount = result
            .Results.Single()
            .GeneratedSources.Count(source =>
                StringComparer.Ordinal.Equals(x: source.HintName, y: AssemblySettingsHintName)
            );

        Assert.Equal(expected: 1, actual: assemblySettingsCount);
    }

    [Fact]
    public void NoNamespaceDeclaration_GeneratesNothing()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new TestAssemblyCodeGenerator(),
            source: """
            public sealed class Example { }
            """
        );

        Assert.Empty(result.Results.Single().GeneratedSources);
    }
}
