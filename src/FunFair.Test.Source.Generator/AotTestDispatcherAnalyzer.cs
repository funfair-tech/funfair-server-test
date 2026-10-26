using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AotTestDispatcherAnalyzer : DiagnosticAnalyzer
{
    private const string CATEGORY = "AotDiscoverability";

    private const string BUILDER_METHOD_NAME = "BuildDispatcherCases";

    private const string CASE_METHOD_NAME = "Case";

    private static readonly DiagnosticDescriptor MissingDispatcherRule = new(
        id: "FTS001",
        title: "Missing AOT test dispatcher",
        messageFormat: "Class '{0}' derives from '{1}' but does not declare an AOT test dispatcher "
            + "([Theory] + [MemberData] method with an Action<{0}> second parameter) - inherited [DispatchedTest] "
            + "methods will not be run",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor IncompleteDispatcherRule = new(
        id: "FTS002",
        title: "Incomplete AOT test dispatcher",
        messageFormat: "Class '{0}' has an AOT test dispatcher but its case list is missing an entry for "
            + "inherited test method '{1}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor IncompleteCaseBuilderRule = new(
        id: "FTS003",
        title: "Incomplete AOT test dispatcher case builder",
        messageFormat: "'{0}."
            + BUILDER_METHOD_NAME
            + "' does not list a case for [DispatchedTest] method '{1}' - "
            + "leaf classes delegating to it will not run that test",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor MissingParentCaseBuilderSpreadRule = new(
        id: "FTS004",
        title: "AOT test dispatcher case builder does not include its parent's cases",
        messageFormat: "'{0}."
            + BUILDER_METHOD_NAME
            + "' does not spread '{1}."
            + BUILDER_METHOD_NAME
            + "' - "
            + "leaf classes delegating to it will not run the inherited [DispatchedTest] methods",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [MissingDispatcherRule, IncompleteDispatcherRule, IncompleteCaseBuilderRule, MissingParentCaseBuilderSpreadRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        WellKnownSymbols symbols = new(context.Compilation);

        // Without [DispatchedTest] no base type can have dispatched tests, so neither check can ever report.
        if (symbols.DispatchedTestAttribute is null)
        {
            return;
        }

        DispatchedTestNameCache dispatchedTestNamesByBaseType = new();

        // Not gated on IsComplete: the base classes' own compilation (FunFair.Test.Common) is where this check has to
        // run, as consumers only see the builders as metadata, and it does not necessarily reference the xunit types
        // the leaf-class checks below need.
        context.RegisterSyntaxNodeAction(
            nodeContext =>
                AnalyzeDispatcherCaseBuilder(
                    nodeContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
                ),
            SyntaxKind.MethodDeclaration
        );

        if (!symbols.IsComplete)
        {
            return;
        }

        context.RegisterSymbolAction(
            symbolContext =>
                AnalyzeClass(
                    symbolContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
                ),
            SymbolKind.NamedType
        );
    }

    private static void AnalyzeClass(
        in SymbolAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        if (
            context.Symbol
            is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsSealed: true, IsAbstract: false } classSymbol
        )
        {
            return;
        }

        IReadOnlyList<INamedTypeSymbol> baseChain = CollectBaseChain(classSymbol);

        INamedTypeSymbol? affectedBaseType = baseChain.FirstOrDefault(bt =>
            !dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols).IsEmpty
        );

        if (affectedBaseType is null)
        {
            return;
        }

        IMethodSymbol? dispatcher = FindDispatcher(classSymbol: classSymbol, symbols: symbols);

        if (dispatcher is null)
        {
            ReportMissingDispatcher(context, classSymbol: classSymbol, affectedBaseType: affectedBaseType);
            return;
        }

        ReportIncompleteDispatcherCases(
            context,
            classSymbol: classSymbol,
            dispatcher: dispatcher,
            baseChain: baseChain,
            symbols: symbols,
            dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
        );
    }

    private static void AnalyzeDispatcherCaseBuilder(
        in SyntaxNodeAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        if (context.Node is not MethodDeclarationSyntax { Identifier.Text: BUILDER_METHOD_NAME } builderSyntax)
        {
            return;
        }

        if (
            context.SemanticModel.GetDeclaredSymbol(builderSyntax, context.CancellationToken)
            is not { IsStatic: true, ContainingType: { } typeSymbol }
        )
        {
            return;
        }

        Location location = builderSyntax.Identifier.GetLocation();

        ReportMissingParentCaseBuilderSpread(
            context,
            typeSymbol: typeSymbol,
            builderSyntax: builderSyntax,
            location: location
        );

        ImmutableArray<string> dispatchedTestNames = dispatchedTestNamesByBaseType.GetOrAdd(
            baseType: typeSymbol,
            symbols: symbols
        );

        if (dispatchedTestNames.IsEmpty)
        {
            return;
        }

        HashSet<string> caseNames = CollectCaseNames(builderSyntax);

        foreach (string testName in dispatchedTestNames.Where(testName => !caseNames.Contains(testName)))
        {
            context.ReportDiagnostic(Diagnostic.Create(IncompleteCaseBuilderRule, location, typeSymbol.Name, testName));
        }
    }

    // A parent's [DispatchedTest] methods are covered by spreading the parent's own builder, which is checked
    // against those methods where the parent itself is compiled; so a derived builder only has to list the methods
    // it declares, provided it spreads the nearest ancestor builder.
    private static void ReportMissingParentCaseBuilderSpread(
        in SyntaxNodeAnalysisContext context,
        INamedTypeSymbol typeSymbol,
        MethodDeclarationSyntax builderSyntax,
        Location location
    )
    {
        INamedTypeSymbol? parentWithBuilder = FindNearestAncestorWithCaseBuilder(typeSymbol);

        if (parentWithBuilder is null)
        {
            return;
        }

        bool spreadsParentBuilder = builderSyntax
            .DescendantNodes()
            .OfType<SpreadElementSyntax>()
            .Any(spread =>
                spread.Expression is InvocationExpressionSyntax invocation
                && StringComparer.Ordinal.Equals(ExtractSimpleName(invocation.Expression), BUILDER_METHOD_NAME)
            );

        if (!spreadsParentBuilder)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(MissingParentCaseBuilderSpreadRule, location, typeSymbol.Name, parentWithBuilder.Name)
            );
        }
    }

    private static INamedTypeSymbol? FindNearestAncestorWithCaseBuilder(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.GetMembers(BUILDER_METHOD_NAME).OfType<IMethodSymbol>().Any(method => method.IsStatic))
            {
                return current;
            }
        }

        return null;
    }

    private static HashSet<string> CollectCaseNames(MethodDeclarationSyntax builderSyntax)
    {
        HashSet<string> names = new(StringComparer.Ordinal);

        foreach (
            InvocationExpressionSyntax invocation in builderSyntax
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
        )
        {
            if (
                !StringComparer.Ordinal.Equals(ExtractSimpleName(invocation.Expression), CASE_METHOD_NAME)
                || invocation.ArgumentList.Arguments.Count == 0
            )
            {
                continue;
            }

            string? name = ExtractCaseName(invocation.ArgumentList.Arguments[0].Expression.ToString());

            if (name is not null)
            {
                names.Add(name);
            }
        }

        return names;
    }

    // Mirrors DispatcherCaseData.Case's own parsing of its CallerArgumentExpression text, so the name checked here
    // is exactly the name the case is dispatched under at runtime (e.g. "t => t.X().Y()" dispatches as "Y").
    private static string? ExtractCaseName(string expressionText)
    {
        int openParen = expressionText.LastIndexOf('(');

        if (openParen < 0)
        {
            return null;
        }

        int dot = expressionText.LastIndexOf(value: '.', startIndex: openParen);

        return expressionText.Substring(startIndex: dot + 1, length: openParen - dot - 1);
    }

    private static void ReportMissingDispatcher(
        in SymbolAnalysisContext context,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol affectedBaseType
    )
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingDispatcherRule,
                classSymbol.Locations.FirstOrDefault() ?? Location.None,
                classSymbol.Name,
                affectedBaseType.Name
            )
        );
    }

    private static void ReportIncompleteDispatcherCases(
        in SymbolAnalysisContext context,
        INamedTypeSymbol classSymbol,
        IMethodSymbol dispatcher,
        IReadOnlyList<INamedTypeSymbol> baseChain,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        IReadOnlyCollection<string> baseDispatchedTestNames =
        [
            .. baseChain
                .SelectMany(bt => dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols))
                .Distinct(StringComparer.Ordinal),
        ];

        HashSet<string>? declaredCaseNames = TryGetLocallyDeclaredCaseNames(
            classSymbol: classSymbol,
            dispatcher: dispatcher,
            symbols: symbols,
            cancellationToken: context.CancellationToken
        );

        if (declaredCaseNames is null)
        {
            return;
        }

        foreach (string testName in baseDispatchedTestNames.Where(testName => !declaredCaseNames.Contains(testName)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    IncompleteDispatcherRule,
                    dispatcher.Locations.FirstOrDefault() ?? Location.None,
                    classSymbol.Name,
                    testName
                )
            );
        }
    }

    // Every base type is walked, not just FunFair.Test.Common's: a consumer's own intermediate base carrying
    // [DispatchedTest] methods is just as invisible to AOT discovery, and an intermediate between the leaf and a
    // FunFair.Test.Common base must not hide the inherited methods from the check.
    private static IReadOnlyList<INamedTypeSymbol> CollectBaseChain(INamedTypeSymbol classSymbol)
    {
        List<INamedTypeSymbol> chain = [];

        for (
            INamedTypeSymbol? current = classSymbol.BaseType;
            current is not null && current.SpecialType != SpecialType.System_Object;
            current = current.BaseType
        )
        {
            chain.Add(current);
        }

        return chain;
    }

    private static bool IsAttributeOfType(AttributeData attribute, INamedTypeSymbol? attributeType)
    {
        return attributeType is not null
            && SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType);
    }

    private static IMethodSymbol? FindDispatcher(INamedTypeSymbol classSymbol, WellKnownSymbols symbols)
    {
        return classSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(method =>
                IsDispatcherShape(method: method, classSymbol: classSymbol, symbols: symbols)
                && IsTheoryWithMemberData(method: method, symbols: symbols)
            );
    }

    private static bool IsTheoryWithMemberData(IMethodSymbol method, WellKnownSymbols symbols)
    {
        ImmutableArray<AttributeData> attributes = method.GetAttributes();

        return attributes.Any(a => IsAttributeOfType(a, symbols.TheoryAttribute))
            && attributes.Any(a => IsAttributeOfType(a, symbols.MemberDataAttribute));
    }

    private static bool IsDispatcherShape(IMethodSymbol method, INamedTypeSymbol classSymbol, WellKnownSymbols symbols)
    {
        if (method.Parameters.Length != 2)
        {
            return false;
        }

        if (method.Parameters[0].Type.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        if (method.Parameters[1].Type is not INamedTypeSymbol { TypeArguments.Length: 1 } actionType)
        {
            return false;
        }

        bool isActionDelegate = SymbolEqualityComparer.Default.Equals(
            actionType.OriginalDefinition,
            symbols.ActionType
        );

        return isActionDelegate && SymbolEqualityComparer.Default.Equals(actionType.TypeArguments[0], classSymbol);
    }

    private static HashSet<string>? TryGetLocallyDeclaredCaseNames(
        INamedTypeSymbol classSymbol,
        IMethodSymbol dispatcher,
        WellKnownSymbols symbols,
        CancellationToken cancellationToken
    )
    {
        AttributeData? memberDataAttribute = dispatcher
            .GetAttributes()
            .FirstOrDefault(a => IsAttributeOfType(a, symbols.MemberDataAttribute));

        if (memberDataAttribute is null || memberDataAttribute.ConstructorArguments.Length == 0)
        {
            return null;
        }

        if (memberDataAttribute.ConstructorArguments[0].Value is not string memberName)
        {
            return null;
        }

        INamedTypeSymbol providerType =
            memberDataAttribute
                .NamedArguments.FirstOrDefault(namedArg => StringComparer.Ordinal.Equals(namedArg.Key, "MemberType"))
                .Value.Value as INamedTypeSymbol
            ?? classSymbol;

        bool isLocal =
            SymbolEqualityComparer.Default.Equals(providerType, classSymbol)
            || SymbolEqualityComparer.Default.Equals(providerType.ContainingType, classSymbol);

        if (!isLocal)
        {
            return null;
        }

        if (providerType.GetMembers(memberName).OfType<IMethodSymbol>().FirstOrDefault() is not { } providerMethod)
        {
            return null;
        }

        HashSet<string> names = CollectNameofTargets(providerMethod, cancellationToken);

        return names.Count == 0 ? null : names;
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0045:Use GetSyntaxAsync instead of GetSyntax and make method async",
        Justification = "Diagnostic analyzer symbol actions must be synchronous"
    )]
    private static HashSet<string> CollectNameofTargets(
        IMethodSymbol providerMethod,
        CancellationToken cancellationToken
    )
    {
        HashSet<string> names = new(StringComparer.Ordinal);

        foreach (SyntaxReference syntaxRef in providerMethod.DeclaringSyntaxReferences)
        {
            SyntaxNode node = syntaxRef.GetSyntax(cancellationToken);

            foreach (
                InvocationExpressionSyntax invocation in node.DescendantNodesAndSelf()
                    .OfType<InvocationExpressionSyntax>()
            )
            {
                if (invocation.Expression is not IdentifierNameSyntax { Identifier.Text: "nameof" })
                {
                    continue;
                }

                if (invocation.ArgumentList.Arguments.Count != 1)
                {
                    continue;
                }

                string? name = ExtractSimpleName(invocation.ArgumentList.Arguments[0].Expression);

                if (name is not null)
                {
                    names.Add(name);
                }
            }
        }

        return names;
    }

    private static string? ExtractSimpleName(ExpressionSyntax expression)
    {
        return expression switch
        {
            SimpleNameSyntax name => name.Identifier.Text,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            _ => null,
        };
    }

    private sealed class WellKnownSymbols
    {
        public WellKnownSymbols(Compilation compilation)
        {
            this.DispatchedTestAttribute = compilation.GetTypeByMetadataName(
                "FunFair.Test.Common.DispatchedTestAttribute"
            );
            this.TheoryAttribute = compilation.GetTypeByMetadataName("Xunit.TheoryAttribute");
            this.MemberDataAttribute = compilation.GetTypeByMetadataName("Xunit.MemberDataAttribute");
            this.ActionType = compilation.GetTypeByMetadataName("System.Action`1");
        }

        public INamedTypeSymbol? DispatchedTestAttribute { get; }

        public INamedTypeSymbol? TheoryAttribute { get; }

        public INamedTypeSymbol? MemberDataAttribute { get; }

        public INamedTypeSymbol? ActionType { get; }

        public bool IsComplete =>
            this.TheoryAttribute is not null && this.MemberDataAttribute is not null && this.ActionType is not null;
    }

    private sealed class DispatchedTestNameCache
    {
        private ImmutableDictionary<INamedTypeSymbol, ImmutableArray<string>> _cache = ImmutableDictionary.Create<
            INamedTypeSymbol,
            ImmutableArray<string>
        >(SymbolEqualityComparer.Default);

        public ImmutableArray<string> GetOrAdd(INamedTypeSymbol baseType, WellKnownSymbols symbols)
        {
            return ImmutableInterlocked.GetOrAdd(
                location: ref this._cache,
                key: baseType.OriginalDefinition,
                valueFactory: static (key, s) =>
                    [
                        .. key.GetMembers()
                            .OfType<IMethodSymbol>()
                            .Where(m => IsDispatchedTest(method: m, symbols: s))
                            .Select(m => m.Name)
                            .Distinct(StringComparer.Ordinal),
                    ],
                factoryArgument: symbols
            );
        }

        private static bool IsDispatchedTest(IMethodSymbol method, WellKnownSymbols symbols)
        {
            return method.GetAttributes().Any(a => IsAttributeOfType(a, symbols.DispatchedTestAttribute));
        }
    }
}
