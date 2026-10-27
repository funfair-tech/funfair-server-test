using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

public sealed class ProtectedMemberSuppressionGeneratorTests : TestBase
{
    private const string SUPPRESSIONS_HINT_NAME = "ProtectedMemberSuppressions.generated.cs";

    private static readonly IncrementalStepRunReason[] ReusedReasons =
    [
        IncrementalStepRunReason.Cached,
        IncrementalStepRunReason.Unchanged,
    ];

    [Theory]
    [InlineData(
        """
            namespace Sample;

            public abstract class Example
            {
                protected void DoSomething() { }
            }
            """,
        "DoSomething"
    )]
    [InlineData(
        """
            namespace Sample;

            public abstract class Example
            {
                protected void FirstMethod() { }

                protected void SecondMethod() { }
            }
            """,
        "FirstMethod,SecondMethod"
    )]
    [InlineData(
        """
            namespace Sample;

            public abstract class Example
            {
                protected internal void ProtectedInternalMethod() { }
            }
            """,
        "ProtectedInternalMethod"
    )]
    [InlineData(
        """
            namespace Sample;

            public abstract class Example
            {
                private protected void PrivateProtectedMethod() { }
            }
            """,
        "PrivateProtectedMethod"
    )]
    // CA1822: unlike the [Fact] methods in this suite, these [Theory] methods use only their parameters and
    // GeneratorTestHelpers, so the analyzer requires static rather than the TestBase-instance convention.
    public static void ClassWithProtectedMembers_GeneratesSuppressionForEach(
        string source,
        string expectedMemberNamesCsv
    )
    {
        string[] expectedMemberNames = expectedMemberNamesCsv.Split(',');

        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new ProtectedMemberSuppressionGenerator(),
            source: source
        );

        GeneratedSourceResult generated = Assert.Single(
            result.Results.Single().GeneratedSources,
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: SUPPRESSIONS_HINT_NAME)
        );

        string text = generated.SourceText.ToString();

        Assert.Contains("UnusedMember.Global", text, StringComparison.Ordinal);

        string[] suppressionLines =
        [
            .. text.Split(separator: '\n').Where(line => line.Contains("SuppressMessage", StringComparison.Ordinal)),
        ];

        Assert.Equal(expected: expectedMemberNames.Length, actual: suppressionLines.Length);

        foreach (string memberName in expectedMemberNames)
        {
            Assert.Contains(
                suppressionLines,
                line => line.Contains($"Target = \"~M:Sample.Example.{memberName}", StringComparison.Ordinal)
            );
        }
    }

    [Fact]
    public void ClassWithProtectedProperty_GeneratesSuppressionForPropertyAndAccessors()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new ProtectedMemberSuppressionGenerator(),
            source: """
            namespace Sample;

            public abstract class Example
            {
                protected int Value { get; set; }
            }
            """
        );

        GeneratedSourceResult generated = Assert.Single(
            result.Results.Single().GeneratedSources,
            predicate: source => StringComparer.Ordinal.Equals(x: source.HintName, y: SUPPRESSIONS_HINT_NAME)
        );

        string text = generated.SourceText.ToString();

        Assert.Contains("Target = \"~P:Sample.Example.Value", text, StringComparison.Ordinal);
        Assert.Contains("Target = \"~M:Sample.Example.get_Value", text, StringComparison.Ordinal);
        Assert.Contains("Target = \"~M:Sample.Example.set_Value(System.Int32)", text, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(
        """
            namespace Sample;

            public sealed class Example
            {
                protected void DoSomething() { }
            }
            """
    )]
    [InlineData(
        """
            namespace Sample;

            public abstract class Example
            {
                public void DoSomethingPublic() { }

                private void DoSomethingPrivate() { }
            }
            """
    )]
    // CA1822: see the comment on ClassWithProtectedMembers_GeneratesSuppressionForEach above.
    public static void NoEligibleProtectedMembers_GeneratesNothing(string source)
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new ProtectedMemberSuppressionGenerator(),
            source: source
        );

        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void GeneratorsDisabledByBuildProperty_GeneratesNothing()
    {
        GeneratorDriverRunResult result = GeneratorTestHelpers.RunGenerator(
            generator: new ProtectedMemberSuppressionGenerator(),
            source: """
            namespace Sample;

            public abstract class Example
            {
                protected void DoSomething() { }
            }
            """,
            globalOptions: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.FunFairTestDisableSourceGenerators"] = "true",
            }
        );

        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void RerunAfterUnrelatedEdit_ReusesCollectedMembersAndGeneratedSource()
    {
        // A sealed class fails the predicate, so the edit adds nothing to the pipeline; it only forces a new compilation.
        GeneratorRunResult result = GeneratorTestHelpers
            .RerunGeneratorAfterAddingSource(
                generator: new ProtectedMemberSuppressionGenerator(),
                source: """
                namespace Sample;

                public abstract class Example
                {
                    protected void DoSomething() { }
                }
                """,
                addedSource: """
                namespace Sample;

                public sealed class Unrelated
                {
                }
                """
            )
            .Results.Single();

        AssertAllOutputsReused(result.TrackedSteps[ProtectedMemberSuppressionGenerator.CollectProtectedMembersStep]);
        AssertAllOutputsReused(result.TrackedOutputSteps[WellKnownGeneratorOutputs.SourceOutput]);

        Assert.Contains(
            result.GeneratedSources,
            source => StringComparer.Ordinal.Equals(x: source.HintName, y: SUPPRESSIONS_HINT_NAME)
        );
    }

    private static void AssertAllOutputsReused(in ImmutableArray<IncrementalGeneratorRunStep> steps)
    {
        IncrementalStepRunReason[] reasons =
        [
            .. steps.SelectMany(step => step.Outputs).Select(output => output.Reason),
        ];

        Assert.NotEmpty(reasons);
        Assert.All(reasons, reason => Assert.Contains(reason, ReusedReasons));
    }
}
