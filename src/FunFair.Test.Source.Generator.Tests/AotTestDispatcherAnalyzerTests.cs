using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests;

public sealed class AotTestDispatcherAnalyzerTests : TestBase
{
    [Fact]
    public async Task SealedClassDerivingFromAffectedBaseWithNoDispatcher_ReportsMissingDispatcher()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task SealedClassWithCompleteDispatcher_ReportsNothing()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task SealedClassWithIncompleteDispatcher_ReportsMissingCase()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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

        Diagnostic diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expected: "FTS002", actual: diagnostic.Id);
        Assert.Contains("FactTwo", diagnostic.GetMessage(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task DispatcherWithProviderUsingStringLiteralCaseNames_AbstainsFromCompletenessCheck()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task AbstractClassDerivingFromAffectedBase_ReportsNothing()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task SealedClassUnrelatedToAffectedBases_ReportsNothing()
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
    public async Task ConsumerOwnedIntermediateBetweenLeafAndAffectedBase_ReportsNothing()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task DispatcherWithProviderDeclaredOnAffectedBase_AbstainsFromCompletenessCheck()
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

                    [Xunit.Fact]
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
    public async Task DispatcherWithProviderDeclaredOnUnrelatedExternalType_AbstainsFromCompletenessCheck()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task CompletenessCheck_IncludesFactsFromEntireFunFairTestCommonChain()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
                    public void FactOne() { }
                }

                public abstract class ComparableObjectTestBase<T> : EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task DispatcherWithProviderDeclaredOnNestedTypeWithinLeaf_ChecksCompleteness()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task CompilationWithoutXunitReference_ReportsNothing()
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
    public async Task AffectedBaseWithNoFactOrTheoryMethods_ReportsNothing()
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
    public async Task CandidateMethodsWithWrongDispatcherShape_AreNotRecognisedAsDispatcher()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task DispatcherWithNullMemberDataName_AbstainsFromCompletenessCheck()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task DispatcherWithMemberDataNamingNonExistentMember_AbstainsFromCompletenessCheck()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
    public async Task ProviderMethodWithNonSimpleNameofTarget_IgnoresThatTargetWithoutAffectingRealNames()
    {
        ImmutableArray<Diagnostic> diagnostics = await GeneratorTestHelpers.RunAnalyzerAsync(
            analyzer: new AotTestDispatcherAnalyzer(),
            source: """
            namespace FunFair.Test.Common
            {
                public abstract class EquatableObjectTestBase<T>
                {
                    [Xunit.Fact]
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
}
