using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AotTestDispatcherAnalyzer : DiagnosticAnalyzer
{
    private const string CATEGORY = "AotDiscoverability";

    private static readonly ImmutableHashSet<string> AffectedBaseClassNames = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "ComparableObjectTestBase",
        "ComparableValueTestBase",
        "EquatableObjectTestBase",
        "EquatableValueTestBase",
        "JsonConverterObjectTestBase",
        "JsonConverterValueTestBase",
        "ValidatorTestBase"
    );

    private static readonly DiagnosticDescriptor MissingDispatcherRule = new(
        id: "FTS001",
        title: "Missing AOT test dispatcher",
        messageFormat: "Class '{0}' derives from '{1}' but does not declare an AOT test dispatcher "
            + "([Theory] + [MemberData] method with an Action<{0}> second parameter) - inherited [Fact]/[Theory] "
            + "methods will not be discoverable under AOT test discovery",
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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [MissingDispatcherRule, IncompleteDispatcherRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        WellKnownSymbols symbols = new(context.Compilation);

        if (!symbols.IsComplete)
        {
            return;
        }

        FactNameCache factNamesByBaseType = new();

        context.RegisterSymbolAction(
            symbolContext => AnalyzeClass(symbolContext, symbols: symbols, factNamesByBaseType: factNamesByBaseType),
            SymbolKind.NamedType
        );
    }

    private static void AnalyzeClass(
        in SymbolAnalysisContext context,
        WellKnownSymbols symbols,
        FactNameCache factNamesByBaseType
    )
    {
        if (
            context.Symbol
            is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsSealed: true, IsAbstract: false } classSymbol
        )
        {
            return;
        }

        IReadOnlyList<INamedTypeSymbol> commonBaseChain = CollectFunFairTestCommonBaseChain(classSymbol);

        INamedTypeSymbol? affectedBaseType = commonBaseChain.FirstOrDefault(bt =>
            AffectedBaseClassNames.Contains(bt.OriginalDefinition.Name)
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
            baseChain: commonBaseChain,
            symbols: symbols,
            factNamesByBaseType: factNamesByBaseType
        );
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
        FactNameCache factNamesByBaseType
    )
    {
        IReadOnlyCollection<string> baseFactNames =
        [
            .. baseChain
                .SelectMany(bt => factNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols))
                .Distinct(StringComparer.Ordinal),
        ];

        if (baseFactNames.Count == 0)
        {
            return;
        }

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

        foreach (string factName in baseFactNames.Where(factName => !declaredCaseNames.Contains(factName)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    IncompleteDispatcherRule,
                    dispatcher.Locations.FirstOrDefault() ?? Location.None,
                    classSymbol.Name,
                    factName
                )
            );
        }
    }

    private static IReadOnlyList<INamedTypeSymbol> CollectFunFairTestCommonBaseChain(INamedTypeSymbol classSymbol)
    {
        List<INamedTypeSymbol> chain = [];
        INamedTypeSymbol? current = classSymbol.BaseType;

        while (current is not null && IsFunFairTestCommonType(current))
        {
            chain.Add(current);
            current = current.BaseType;
        }

        return chain;
    }

    private static bool IsFunFairTestCommonType(INamedTypeSymbol type)
    {
        return StringComparer.Ordinal.Equals(
            type.OriginalDefinition.ContainingNamespace?.ToDisplayString(),
            "FunFair.Test.Common"
        );
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
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            _ => null,
        };
    }

    private sealed class WellKnownSymbols
    {
        public WellKnownSymbols(Compilation compilation)
        {
            this.FactAttribute = compilation.GetTypeByMetadataName("Xunit.FactAttribute");
            this.TheoryAttribute = compilation.GetTypeByMetadataName("Xunit.TheoryAttribute");
            this.MemberDataAttribute = compilation.GetTypeByMetadataName("Xunit.MemberDataAttribute");
            this.ActionType = compilation.GetTypeByMetadataName("System.Action`1");
        }

        public INamedTypeSymbol? FactAttribute { get; }

        public INamedTypeSymbol? TheoryAttribute { get; }

        public INamedTypeSymbol? MemberDataAttribute { get; }

        public INamedTypeSymbol? ActionType { get; }

        public bool IsComplete =>
            this.FactAttribute is not null
            && this.TheoryAttribute is not null
            && this.MemberDataAttribute is not null
            && this.ActionType is not null;
    }

    private sealed class FactNameCache
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
                            .Where(m => IsFactOrTheory(method: m, symbols: s))
                            .Select(m => m.Name)
                            .Distinct(StringComparer.Ordinal),
                    ],
                factoryArgument: symbols
            );
        }

        private static bool IsFactOrTheory(IMethodSymbol method, WellKnownSymbols symbols)
        {
            return method
                .GetAttributes()
                .Any(a => IsAttributeOfType(a, symbols.FactAttribute) || IsAttributeOfType(a, symbols.TheoryAttribute));
        }
    }
}
