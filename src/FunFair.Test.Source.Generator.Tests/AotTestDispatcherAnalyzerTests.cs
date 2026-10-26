using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

public sealed class AotTestDispatcherAnalyzerTests : TestBase
{
    [Fact]
    public async Task SealedClassDerivingFromAffectedBaseWithNoDispatcher_ReportsMissingDispatcherAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS001", actual: diagnostic.Id);
        Assert.Contains("Leaf", diagnostic.GetMessage(), StringComparison.Ordinal);
        Assert.Contains("EquatableObjectTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task SealedClassDerivingFromConsumerBaseWithDispatchedTestsAndNoDispatcher_ReportsMissingDispatcherAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace Sample
            {
                public abstract class CustomTestBase
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }

                public sealed class Leaf : CustomTestBase
                {
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS001", actual: diagnostic.Id);
        Assert.Contains("Leaf", diagnostic.GetMessage(), StringComparison.Ordinal);
        Assert.Contains("CustomTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task SealedClassDerivingFromUnlistedFunFairBaseWithDispatchedTestsAndNoDispatcher_ReportsMissingDispatcherAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class SomeNewTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                public sealed class Leaf : FunFair.Test.Common.SomeNewTestBase<string>
                {
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS001", actual: diagnostic.Id);
        Assert.Contains("Leaf", diagnostic.GetMessage(), StringComparison.Ordinal);
        Assert.Contains("SomeNewTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task SealedClassDerivingFromBaseWithNoDispatchedTestsAndNoDispatcher_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
                    public void NotDispatched() { }
                }
            }

            namespace Sample
            {
                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task SealedClassWithCompleteDispatcher_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return [nameof(FactOne), (Action<Leaf>)(t => { })];
                        yield return [nameof(FactTwo), (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task SealedClassWithIncompleteDispatcher_ReportsMissingCaseAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return [nameof(FactOne), (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS002", actual: diagnostic.Id);
        Assert.Contains("FactTwo", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task BaseMethodWithXunitFactButNoDispatchedTestMarker_IsNotRequiredInCaseListAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [Xunit.Fact]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return [nameof(FactOne), (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DispatcherWithProviderUsingStringLiteralCaseNames_AbstainsFromCompletenessCheckAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return ["FactOne", (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task AbstractClassDerivingFromAffectedBase_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                public abstract class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task SealedClassUnrelatedToAffectedBases_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace Sample;

            public sealed class Leaf
            {
                [Xunit.Fact]
                public void SomeTest() { }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ConsumerOwnedIntermediateBetweenLeafAndAffectedBase_ReportsMissingDispatcherAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                public abstract class ConsumerOwnedIntermediate : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                }

                public sealed class Leaf : ConsumerOwnedIntermediate
                {
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS001", actual: diagnostic.Id);
        Assert.Contains("EquatableObjectTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task DispatcherWithProviderDeclaredOnAffectedBase_AbstainsFromCompletenessCheckAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System.Collections.Generic;

                public abstract class EquatableObjectTestBase<T>
                {
                    public static IEnumerable<object[]> BaseCases()
                    {
                        yield return [nameof(FactOne)];
                    }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    [Theory]
                    [MemberData(
                        nameof(FunFair.Test.Common.EquatableObjectTestBase<string>.BaseCases),
                        MemberType = typeof(FunFair.Test.Common.EquatableObjectTestBase<string>)
                    )]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DispatcherWithProviderDeclaredOnUnrelatedExternalType_AbstainsFromCompletenessCheckAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public static class Unrelated
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return [nameof(Leaf.FactOne)];
                    }
                }

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    [Theory]
                    [MemberData(nameof(Unrelated.Cases), MemberType = typeof(Unrelated))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task CompletenessCheck_IncludesFactsFromEntireFunFairTestCommonChainAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }

                public abstract class ComparableObjectTestBase<T> : EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.ComparableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield return [nameof(FactTwo), (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS002", actual: diagnostic.Id);
        Assert.Contains("FactOne", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task DispatcherWithProviderDeclaredOnNestedTypeWithinLeaf_ChecksCompletenessAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static class Cases
                    {
                        public static IEnumerable<object[]> All()
                        {
                            yield return [nameof(Leaf.FactOne), (Action<Leaf>)(t => { })];
                        }
                    }

                    [Theory]
                    [MemberData(nameof(Cases.All), MemberType = typeof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS002", actual: diagnostic.Id);
        Assert.Contains("FactTwo", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task CompilationWithoutXunitReference_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerWithoutXunitReferenceAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace Sample;

            public sealed class Leaf
            {
                public void SomeMethod() { }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task AffectedBaseWithNoDispatchedTestMethods_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        yield break;
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task CandidateMethodsWithWrongDispatcherShape_AreNotRecognisedAsDispatcherAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public void FirstParamIsNotAString(int name, Action<Leaf> action) { }

                    public void SecondParamIsNotAnAction(string name, object notAnAction) { }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS001", actual: diagnostic.Id);
    }

    [Fact]
    public async Task DispatcherWithNullMemberDataName_AbstainsFromCompletenessCheckAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    [Theory]
                    [MemberData(null)]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DispatcherWithMemberDataNamingNonExistentMember_AbstainsFromCompletenessCheckAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    [Theory]
                    [MemberData("NoSuchMember")]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ProviderMethodWithNonSimpleNameofTarget_IgnoresThatTargetWithoutAffectingRealNamesAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }
                }
            }

            namespace Sample
            {
                using System;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Leaf : FunFair.Test.Common.EquatableObjectTestBase<string>
                {
                    public static IEnumerable<object[]> Cases()
                    {
                        _ = nameof(List<int>);
                        yield return [nameof(FactOne), (Action<Leaf>)(t => { })];
                    }

                    [Theory]
                    [MemberData(nameof(Cases))]
                    public void CommonTests(string name, Action<Leaf> action) { }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task BaseBuilderMissingDispatchedTestCase_ReportsIncompleteBuilderAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;
                using static FunFair.Test.Common.DispatcherCaseData;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }

                    public void NotDispatched() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T>
                    {
                        return [Case<TSelf>(t => t.FactOne()), Case<TSelf>(t => t.NotDispatched())];
                    }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS003", actual: diagnostic.Id);
        Assert.Contains("EquatableObjectTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
        Assert.Contains("'FactTwo'", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExpressionBodiedBaseBuilderMissingDispatchedTestCase_ReportsIncompleteBuilderAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;

                public abstract class ValidatorTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    protected abstract void EverythingValid();

                    [FunFair.Test.Common.DispatchedTest]
                    protected void SomethingInvalid() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : ValidatorTestBase<T> => [DispatcherCaseData.Case<TSelf>(t => t.EverythingValid())];
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS003", actual: diagnostic.Id);
        Assert.Contains("'SomethingInvalid'", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task BaseBuilderCaseWhoseCallIsNotTheDispatchedTest_ReportsIncompleteBuilderAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;
                using static FunFair.Test.Common.DispatcherCaseData;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public int FactOne() => 1;

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T>
                    {
                        return [Case<TSelf>(t => t.FactOne().ToString(System.Globalization.CultureInfo.InvariantCulture))];
                    }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS003", actual: diagnostic.Id);
        Assert.Contains("'FactOne'", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task CompleteBaseBuilder_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;
                using static FunFair.Test.Common.DispatcherCaseData;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T>
                    {
                        _ = ((Func<int>)(() => 1))();

                        return
                        [
                            Case<TSelf>(t => t.FactOne()),
                            Case<TSelf>(t => { t.FactTwo(); }),
                            Case<TSelf>(null!),
                            ("Unrelated", t => { }),
                        ];
                    }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DerivedBuilderSpreadingParentBuilder_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;
                using static FunFair.Test.Common.DispatcherCaseData;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T>
                    {
                        return [Case<TSelf>(t => t.FactOne())];
                    }
                }

                public abstract class ComparableObjectTestBase<T> : EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }

                    public static new (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : ComparableObjectTestBase<T>
                    {
                        return [.. EquatableObjectTestBase<T>.BuildDispatcherCases<TSelf>(), Case<TSelf>(t => t.FactTwo())];
                    }
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DerivedBuilderNotSpreadingParentBuilder_ReportsMissingParentSpreadAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;
                using static FunFair.Test.Common.DispatcherCaseData;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T>
                    {
                        return [Case<TSelf>(t => t.FactOne())];
                    }
                }

                public abstract class ComparableObjectTestBase<T> : EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactTwo() { }

                    public static new (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : ComparableObjectTestBase<T>
                    {
                        return [.. Array.Empty<(string Name, Action<TSelf> Action)>(), Case<TSelf>(t => t.FactTwo())];
                    }
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS004", actual: diagnostic.Id);
        Assert.Contains("ComparableObjectTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
        Assert.Contains("EquatableObjectTestBase", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task TypeWithBuilderButNoDispatchedTests_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;

                public abstract class JsonConverterObjectTestBase<T>
                {
                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : JsonConverterObjectTestBase<T> => [];
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task IncompleteBaseBuilderWithoutXunitReference_StillReportsIncompleteBuilderAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerWithoutXunitReferenceAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                using System;

                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : EquatableObjectTestBase<T> => [];
                }
            }
            """
        );

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS003", actual: diagnostic.Id);
    }

    [Fact]
    public async Task InstanceMethodNamedLikeBuilder_IsNotTreatedAsCaseBuilderAsync()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [FunFair.Test.Common.DispatchedTest]
                    public void FactOne() { }

                    public int BuildDispatcherCases() => 0;
                }
            }
            """
        );

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task CompilationWithoutFunFairTestCommonReference_WithSourceDeclaredDispatchedTestAttribute_ReportsBothChecksAsync()
    {
        // Control for the test below: the same source does fire both checks once
        // FunFair.Test.Common.DispatchedTestAttribute resolves, so that test's silence is down to the attribute alone.
        ImmutableArray<Diagnostic> diagnostics =
            await GeneratorTestHelpers.RunAnalyzerWithoutFunFairTestCommonReferenceAsync(
                analyzer: new AotTestDispatcherAnalyzer(),
                source: BuildSourceWithDispatchedTestAttributeIn("FunFair.Test.Common")
            );

        string[] expectedIds = ["FTS001", "FTS003"];
        Assert.Equal(
            expected: expectedIds,
            actual: diagnostics.Select(d => d.Id).Order(StringComparer.Ordinal),
            comparer: StringComparer.Ordinal
        );
    }

    [Fact]
    public async Task CompilationWithoutDispatchedTestAttribute_ReportsNothingAsync()
    {
        ImmutableArray<Diagnostic> diagnostics =
            await GeneratorTestHelpers.RunAnalyzerWithoutFunFairTestCommonReferenceAsync(
                analyzer: new AotTestDispatcherAnalyzer(),
                source: BuildSourceWithDispatchedTestAttributeIn("Sample.Unrelated")
            );

        Assert.Empty(diagnostics);
    }

    private static string BuildSourceWithDispatchedTestAttributeIn(string attributeNamespace)
    {
        return $$"""
            namespace {{attributeNamespace}}
            {
                public sealed class DispatchedTestAttribute : System.Attribute
                {
                }
            }

            namespace Sample
            {
                using System;

                public abstract class AffectedBase<T>
                {
                    [{{attributeNamespace}}.DispatchedTest]
                    public void FactOne() { }

                    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
                        where TSelf : AffectedBase<T> => [];
                }

                public sealed class Leaf : AffectedBase<string>
                {
                }
            }
            """;
    }
}
