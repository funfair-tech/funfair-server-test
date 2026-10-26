using System;
using System.Collections.Generic;
using System.Linq;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

public sealed class TestAssemblyCodeGeneratorTests : TestBase
{
    private const string ASSEMBLY_SETTINGS_HINT_NAME = "AssemblySettings.generated.cs";

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
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: ASSEMBLY_SETTINGS_HINT_NAME)
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

        Assert.Single(
            result.Results.Single().GeneratedSources,
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: ASSEMBLY_SETTINGS_HINT_NAME)
        );
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

    [Theory]
    [InlineData("true")]
    [InlineData("True")]
    // CA1822: see the comment on
    // ProtectedMemberSuppressionGeneratorTests.ClassWithProtectedMembers_GeneratesSuppressionForEach.
    public static void GeneratorsDisabledByBuildProperty_GeneratesNothing(string value)
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new TestAssemblyCodeGenerator(),
            source: """
            namespace Sample;

            public sealed class Example { }
            """,
            globalOptions: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.FunFairTestDisableSourceGenerators"] = value,
            }
        );

        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void GeneratorsNotDisabledByBuildProperty_GeneratesAssemblySettings()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new TestAssemblyCodeGenerator(),
            source: """
            namespace Sample;

            public sealed class Example { }
            """,
            globalOptions: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.FunFairTestDisableSourceGenerators"] = "false",
            }
        );

        Assert.Single(
            result.Results.Single().GeneratedSources,
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: ASSEMBLY_SETTINGS_HINT_NAME)
        );
    }
}
