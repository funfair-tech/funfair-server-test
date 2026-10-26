using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using FunFair.Test.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FunFair.Test.Source.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AotTestDispatcherAnalyzer : DiagnosticAnalyzer
{
    private const string CATEGORY = "AotDiscoverability";

    private const string BUILDER_METHOD_NAME = "BuildDispatcherCases";

    private const string CASE_METHOD_NAME = "Case";

    private const string CASE_ACTION_PARAMETER_NAME = "action";

    private const string CASE_EXPRESSION_TEXT_PARAMETER_NAME = "expressionText";

    private const string TO_THEORY_DATA_METHOD_NAME = "ToTheoryData";

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
            + "' does not include '{1}."
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
        // Generated code is analysed, so a [MemberData] provider in a generated partial is still checked against its
        // leaf's dispatcher, but never reported on: a diagnostic located in generated code is dropped, as the user
        // cannot fix it there. Only FTS002 can land outside the code it analyses (on the dispatcher), so the other
        // checks skip generated code outright rather than do work whose diagnostics would be dropped.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
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
        DispatcherCache dispatchersByLeafType = new();

        context.RegisterSyntaxNodeAction(
            nodeContext =>
                AnalyzeMethodDeclaration(
                    nodeContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                    dispatchersByLeafType: dispatchersByLeafType
                ),
            SyntaxKind.MethodDeclaration
        );

        if (!symbols.IsComplete)
        {
            return;
        }

        context.RegisterSyntaxNodeAction(
            nodeContext =>
                AnalyzePropertyDeclaration(
                    nodeContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                    dispatchersByLeafType: dispatchersByLeafType
                ),
            SyntaxKind.PropertyDeclaration
        );

        context.RegisterSyntaxNodeAction(
            nodeContext =>
                AnalyzeFieldDeclaration(
                    nodeContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                    dispatchersByLeafType: dispatchersByLeafType
                ),
            SyntaxKind.FieldDeclaration
        );

        context.RegisterSymbolAction(
            symbolContext =>
                AnalyzeClass(
                    symbolContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                    dispatchersByLeafType: dispatchersByLeafType
                ),
            SymbolKind.NamedType
        );
    }

    // One callback for both method checks, so each static method's symbol is resolved once between them.
    private static void AnalyzeMethodDeclaration(
        in SyntaxNodeAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        // Case builders and [MemberData] providers are both static, so the cheap syntactic check filters out almost
        // every method.
        if (
            context.Node is not MethodDeclarationSyntax methodSyntax
            || !methodSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
            || context.SemanticModel.GetDeclaredSymbol(methodSyntax, context.CancellationToken)
                is not { ContainingType: { } containingType } methodSymbol
        )
        {
            return;
        }

        // Not gated on IsComplete: a builder can only be checked in the compilation that declares it as source, which
        // for FunFair.Test.Common's base classes is that project alone (consumers only see those builders as
        // metadata), and such a compilation, a consumer's included, does not necessarily reference the xunit types
        // the leaf-class checks need.
        if (!context.IsGeneratedCode && IsCaseBuilder(method: methodSymbol, symbols: symbols))
        {
            AnalyzeDispatcherCaseBuilder(
                context,
                builderSyntax: methodSyntax,
                typeSymbol: containingType,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
            );
        }

        // Checked from the provider's own declaration rather than the leaf's symbol action: only a syntax node action
        // has a SemanticModel to resolve each Case(...) through, and Compilation.GetSemanticModel is off-limits in an
        // analyzer (RS1030).
        if (symbols.IsComplete)
        {
            AnalyzeDispatcherCaseProvider(
                context,
                providerSyntax: methodSyntax,
                providerMember: methodSymbol,
                providerType: containingType,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                dispatchersByLeafType: dispatchersByLeafType
            );
        }
    }

    // A [MemberData] provider can equally be a static property or field; an instance one cannot be a provider. Each
    // callback is registered for its one syntax kind, so the node is always of the type it casts to.
    private static void AnalyzePropertyDeclaration(
        in SyntaxNodeAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        PropertyDeclarationSyntax propertySyntax = (PropertyDeclarationSyntax)context.Node;

        if (propertySyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            AnalyzeDataMemberProvider(
                context,
                providerSyntax: propertySyntax,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                dispatchersByLeafType: dispatchersByLeafType
            );
        }
    }

    // A field is resolved per declarator, as that is the node a field's symbol is declared on, and each declarator is
    // its own provider.
    private static void AnalyzeFieldDeclaration(
        in SyntaxNodeAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        FieldDeclarationSyntax fieldSyntax = (FieldDeclarationSyntax)context.Node;

        if (!fieldSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        foreach (VariableDeclaratorSyntax declarator in fieldSyntax.Declaration.Variables)
        {
            AnalyzeDataMemberProvider(
                context,
                providerSyntax: declarator,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                dispatchersByLeafType: dispatchersByLeafType
            );
        }
    }

    private static void AnalyzeDataMemberProvider(
        in SyntaxNodeAnalysisContext context,
        SyntaxNode providerSyntax,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        if (
            context.SemanticModel.GetDeclaredSymbol(providerSyntax, context.CancellationToken) is
            { ContainingType: { } containingType } providerMember
        )
        {
            AnalyzeDispatcherCaseProvider(
                context,
                providerSyntax: providerSyntax,
                providerMember: providerMember,
                providerType: containingType,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                dispatchersByLeafType: dispatchersByLeafType
            );
        }
    }

    private static void AnalyzeClass(
        in SymbolAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        if (context.IsGeneratedCode || context.Symbol is not INamedTypeSymbol classSymbol || !IsLeafClass(classSymbol))
        {
            return;
        }

        INamedTypeSymbol? affectedBaseType = CollectBaseChain(classSymbol)
            .FirstOrDefault(bt => !dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols).IsEmpty);

        if (affectedBaseType is null)
        {
            return;
        }

        if (dispatchersByLeafType.GetOrAdd(classSymbol: classSymbol, symbols: symbols) is null)
        {
            ReportMissingDispatcher(context, classSymbol: classSymbol, affectedBaseType: affectedBaseType);
        }
    }

    private static bool IsLeafClass(INamedTypeSymbol classSymbol)
    {
        return classSymbol is { TypeKind: TypeKind.Class, IsSealed: true, IsAbstract: false };
    }

    private static void AnalyzeDispatcherCaseProvider(
        in SyntaxNodeAnalysisContext context,
        SyntaxNode providerSyntax,
        ISymbol providerMember,
        INamedTypeSymbol providerType,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        // A local provider is declared on the leaf itself or on a type nested directly inside it.
        ReportIncompleteDispatcherCases(
            context,
            classSymbol: providerType,
            providerMember: providerMember,
            providerSyntax: providerSyntax,
            symbols: symbols,
            dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
            dispatchersByLeafType: dispatchersByLeafType
        );

        if (providerType.ContainingType is { } outerType)
        {
            ReportIncompleteDispatcherCases(
                context,
                classSymbol: outerType,
                providerMember: providerMember,
                providerSyntax: providerSyntax,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
                dispatchersByLeafType: dispatchersByLeafType
            );
        }
    }

    private static void AnalyzeDispatcherCaseBuilder(
        in SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax builderSyntax,
        INamedTypeSymbol typeSymbol,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        CaseCompositionShape shape = CaseCompositionShape.Read(
            memberSyntax: builderSyntax,
            semanticModel: context.SemanticModel,
            symbols: symbols,
            cancellationToken: context.CancellationToken
        );

        // Both checks are errors that cannot be suppressed, so a composition they cannot follow (a helper method, a
        // mutated local, an unrecognised LINQ operator, ...) must not fail a build whose cases may well be complete.
        if (!shape.IsRecognised)
        {
            return;
        }

        Location location = builderSyntax.Identifier.GetLocation();

        ReportMissingParentCaseBuilderSpread(
            context,
            typeSymbol: typeSymbol,
            shape: shape,
            location: location,
            symbols: symbols
        );

        ReportIncompleteCaseBuilder(
            context,
            typeSymbol: typeSymbol,
            shape: shape,
            location: location,
            symbols: symbols,
            dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
        );
    }

    private static void ReportIncompleteCaseBuilder(
        in SyntaxNodeAnalysisContext context,
        INamedTypeSymbol typeSymbol,
        CaseCompositionShape shape,
        Location location,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        IReadOnlyList<string> dispatchedTestNames =
        [
            .. CollectBuilderSlice(typeSymbol: typeSymbol, symbols: symbols)
                .SelectMany(type => dispatchedTestNamesByBaseType.GetOrAdd(baseType: type, symbols: symbols))
                .Distinct(StringComparer.Ordinal),
        ];

        // Only an ancestor's builder is known to hold none of this builder's slice's cases (its TSelf constraint is
        // the ancestor, so it cannot call a method declared below it); any other builder composed in (an unrelated
        // type's, or this one's own) could list them, so the check abstains rather than guess.
        if (
            dispatchedTestNames.Count == 0
            || shape.HasUnknowableCaseName
            || !shape.ComposedBuilders.All(builder => IsAncestorOf(ancestor: builder.ContainingType, type: typeSymbol))
        )
        {
            return;
        }

        foreach (string testName in dispatchedTestNames.Where(testName => !shape.ListsCase(testName)))
        {
            context.ReportDiagnostic(Diagnostic.Create(IncompleteCaseBuilderRule, location, typeSymbol.Name, testName));
        }
    }

    // An ancestor builder's [DispatchedTest] methods are covered by composing in that builder, which is checked
    // against them where the ancestor itself is compiled; so a derived builder only has to list its slice's methods
    // (see CollectBuilderSlice), provided it composes in the nearest ancestor builder.
    private static void ReportMissingParentCaseBuilderSpread(
        in SyntaxNodeAnalysisContext context,
        INamedTypeSymbol typeSymbol,
        CaseCompositionShape shape,
        Location location,
        WellKnownSymbols symbols
    )
    {
        INamedTypeSymbol? parentWithBuilder = FindNearestAncestorWithCaseBuilder(
            typeSymbol: typeSymbol,
            symbols: symbols
        );

        if (parentWithBuilder is null)
        {
            return;
        }

        if (!shape.ComposedBuilders.Any(builder => IsBuilderOwnedBy(builder: builder, owner: parentWithBuilder)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(MissingParentCaseBuilderSpreadRule, location, typeSymbol.Name, parentWithBuilder.Name)
            );
        }
    }

    private static INamedTypeSymbol? FindNearestAncestorWithCaseBuilder(
        INamedTypeSymbol typeSymbol,
        WellKnownSymbols symbols
    )
    {
        return CollectBaseChain(typeSymbol).FirstOrDefault(type => HasCaseBuilder(type: type, symbols: symbols));
    }

    // The types whose [DispatchedTest] methods a builder declared on typeSymbol must list itself: its own and those of
    // every ancestor up to, but not including, the nearest one with a builder of its own. Nothing else lists an
    // intermediate without a builder, so leaving it to the ancestor builder would drop its methods from every leaf.
    private static IEnumerable<INamedTypeSymbol> CollectBuilderSlice(
        INamedTypeSymbol typeSymbol,
        WellKnownSymbols symbols
    )
    {
        return CollectBaseChain(typeSymbol)
            .TakeWhile(type => !HasCaseBuilder(type: type, symbols: symbols))
            .Prepend(typeSymbol);
    }

    private static bool HasCaseBuilder(INamedTypeSymbol type, WellKnownSymbols symbols)
    {
        return type.GetMembers(BUILDER_METHOD_NAME)
            .OfType<IMethodSymbol>()
            .Any(method => IsCaseBuilder(method: method, symbols: symbols));
    }

    // A case builder has exactly the shape of every FunFair.Test.Common base class's builder:
    // "static IEnumerable<(string, Action<TSelf>)> BuildDispatcherCases<TSelf>() where TSelf : DeclaringType", with
    // any enumerable return type (an array, a list, an iterator's IEnumerable) and any tuple element names. TSelf
    // being constrained to the declaring type is what lets FTS003 and FTS004 hold it to its own slice of the chain, so
    // anything else of that name (a leaf's [MemberData] provider, say) is not a builder, and is left to FTS002 when it
    // is a provider.
    private static bool IsCaseBuilder(IMethodSymbol method, WellKnownSymbols symbols)
    {
        IMethodSymbol definition = method.OriginalDefinition;

        return definition is { IsStatic: true, Parameters.IsEmpty: true, TypeParameters.Length: 1 }
            && StringComparer.Ordinal.Equals(definition.Name, BUILDER_METHOD_NAME)
            && IsConstrainedTo(typeParameter: definition.TypeParameters[0], type: definition.ContainingType)
            && EnumeratedType(definition.ReturnType) is { } caseType
            && IsCaseTuple(caseType: caseType, selfType: definition.TypeParameters[0], symbols: symbols);
    }

    private static bool IsConstrainedTo(ITypeParameterSymbol typeParameter, INamedTypeSymbol type)
    {
        return typeParameter.ConstraintTypes.Any(constraint =>
            SymbolEqualityComparer.Default.Equals(constraint.OriginalDefinition, type.OriginalDefinition)
        );
    }

    // An interface's AllInterfaces does not include itself, so an IEnumerable<T> return type is matched directly.
    private static ITypeSymbol? EnumeratedType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol { IsSZArray: true } array)
        {
            return array.ElementType;
        }

        INamedTypeSymbol? enumerable =
            type is INamedTypeSymbol named && IsGenericEnumerable(named)
                ? named
                : type.AllInterfaces.FirstOrDefault(IsGenericEnumerable);

        return enumerable?.TypeArguments[0];
    }

    private static bool IsGenericEnumerable(INamedTypeSymbol type)
    {
        return type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
    }

    private static bool IsCaseTuple(ITypeSymbol caseType, ITypeParameterSymbol selfType, WellKnownSymbols symbols)
    {
        return caseType is INamedTypeSymbol { IsTupleType: true, TupleElements.Length: 2 } tupleType
            && tupleType.TupleElements[0].Type.SpecialType == SpecialType.System_String
            && tupleType.TupleElements[1].Type is INamedTypeSymbol actionType
            && SymbolEqualityComparer.Default.Equals(actionType.OriginalDefinition, symbols.ActionType)
            && SymbolEqualityComparer.Default.Equals(actionType.TypeArguments[0], selfType);
    }

    // Resolved rather than matched by name, so a grandparent's, an unrelated type's or the builder's own
    // BuildDispatcherCases (a copy-paste slip that drops the parent's cases or recurses forever) is not mistaken for
    // the parent's.
    private static bool IsBuilderOwnedBy(IMethodSymbol builder, INamedTypeSymbol owner)
    {
        return SymbolEqualityComparer.Default.Equals(
            builder.ContainingType.OriginalDefinition,
            owner.OriginalDefinition
        );
    }

    private static bool IsAncestorOf(INamedTypeSymbol ancestor, INamedTypeSymbol type)
    {
        return CollectBaseChain(type)
            .Any(baseType =>
                SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, ancestor.OriginalDefinition)
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
        in SyntaxNodeAnalysisContext context,
        INamedTypeSymbol classSymbol,
        ISymbol providerMember,
        SyntaxNode providerSyntax,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        DispatcherCache dispatchersByLeafType
    )
    {
        if (!IsLeafClass(classSymbol))
        {
            return;
        }

        IMethodSymbol? dispatcher = dispatchersByLeafType.GetOrAdd(classSymbol: classSymbol, symbols: symbols);

        if (
            dispatcher is null
            || !IsProviderOf(
                dispatcher: dispatcher,
                classSymbol: classSymbol,
                providerMember: providerMember,
                symbols: symbols
            )
        )
        {
            return;
        }

        HashSet<string>? coveredCaseNames = TryGetCoveredCaseNames(
            providerSyntax: providerSyntax,
            semanticModel: context.SemanticModel,
            classSymbol: classSymbol,
            symbols: symbols,
            dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType,
            cancellationToken: context.CancellationToken
        );

        if (coveredCaseNames is null)
        {
            return;
        }

        // A method a composed builder does not cover is reported with the same message as an unlisted one: either way
        // the leaf's case list has no entry for it, and listing it locally fixes both.
        foreach (
            string testName in CollectBaseChain(classSymbol)
                .SelectMany(bt => dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols))
                .Distinct(StringComparer.Ordinal)
                .Where(testName => !coveredCaseNames.Contains(testName))
        )
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

    // The caller only passes the leaf itself or a type nested directly inside it as the provider's owner, so matching
    // the [MemberData] target to this provider also establishes that the provider is local to the leaf.
    private static bool IsProviderOf(
        IMethodSymbol dispatcher,
        INamedTypeSymbol classSymbol,
        ISymbol providerMember,
        WellKnownSymbols symbols
    )
    {
        AttributeData? memberDataAttribute = dispatcher
            .GetAttributes()
            .FirstOrDefault(a => IsAttributeOfType(a, symbols.MemberDataAttribute));

        if (
            memberDataAttribute?.ConstructorArguments.IsEmpty != false
            || memberDataAttribute.ConstructorArguments[0].Value is not string memberName
        )
        {
            return false;
        }

        INamedTypeSymbol providerType =
            memberDataAttribute
                .NamedArguments.FirstOrDefault(namedArg => StringComparer.Ordinal.Equals(namedArg.Key, "MemberType"))
                .Value.Value as INamedTypeSymbol
            ?? classSymbol;

        // Matched by kind, so a method provider is still matched against the first method of that name, as before.
        return SymbolEqualityComparer.Default.Equals(
            providerType.GetMembers(memberName).FirstOrDefault(member => member.Kind == providerMember.Kind),
            providerMember
        );
    }

    // The [DispatchedTest] names the provider's cases cover: those it lists itself, plus those of every builder it
    // composes in (a provider that delegates with "BuildDispatcherCases<TSelf>().ToTheoryData()", or spreads or
    // concatenates it). Null means the cover cannot be worked out from the provider's own source, so the check abstains.
    private static HashSet<string>? TryGetCoveredCaseNames(
        SyntaxNode providerSyntax,
        SemanticModel semanticModel,
        INamedTypeSymbol classSymbol,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType,
        CancellationToken cancellationToken
    )
    {
        CaseCompositionShape shape = CaseCompositionShape.Read(
            memberSyntax: providerSyntax,
            semanticModel: semanticModel,
            symbols: symbols,
            cancellationToken: cancellationToken
        );

        if (!shape.IsRecognised || shape.HasUnknowableCaseName)
        {
            return null;
        }

        HashSet<string> names = new(shape.CaseNames, StringComparer.Ordinal);
        names.UnionWith(CollectNameofTargets(providerSyntax));

        if (shape.ComposedBuilders.Count == 0)
        {
            return names.Count == 0 ? null : names;
        }

        // Only a builder owned by the leaf or one of its ancestors is known to hold nothing but that chain's cases; any
        // other (an unrelated type's) could list anything, so the check abstains rather than guess.
        if (!shape.ComposedBuilders.All(builder => IsBuilderInChainOf(builder: builder, classSymbol: classSymbol)))
        {
            return null;
        }

        names.UnionWith(
            shape.ComposedBuilders.SelectMany(builder =>
                CollectBuilderCover(
                    owner: builder.ContainingType,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
                )
            )
        );

        return names;
    }

    private static bool IsBuilderInChainOf(IMethodSymbol builder, INamedTypeSymbol classSymbol)
    {
        return IsBuilderOwnedBy(builder: builder, owner: classSymbol)
            || IsAncestorOf(ancestor: builder.ContainingType, type: classSymbol);
    }

    // FTS003 and FTS004 hold each builder to listing its slice's [DispatchedTest] methods (its owner's and those of
    // the builder-less ancestors above it) and composing in the nearest ancestor builder, so a builder covers its own
    // slice and, through that chain, every ancestor builder's. A type between the leaf and the owner is not covered,
    // which is how a delegation that skips past a nearer ancestor's [DispatchedTest] methods is caught.
    private static IEnumerable<string> CollectBuilderCover(
        INamedTypeSymbol owner,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        for (
            INamedTypeSymbol? builderOwner = owner;
            builderOwner is not null;
            builderOwner = FindNearestAncestorWithCaseBuilder(typeSymbol: builderOwner, symbols: symbols)
        )
        {
            foreach (INamedTypeSymbol type in CollectBuilderSlice(typeSymbol: builderOwner, symbols: symbols))
            {
                foreach (string testName in dispatchedTestNamesByBaseType.GetOrAdd(baseType: type, symbols: symbols))
                {
                    yield return testName;
                }
            }
        }
    }

    private static IEnumerable<string> CollectNameofTargets(SyntaxNode providerSyntax)
    {
        return providerSyntax
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(IsNameofInvocation)
            .Select(static invocation => ExtractSimpleName(invocation.ArgumentList.Arguments[0].Expression))
            .OfType<string>();
    }

    private static bool IsNameofInvocation(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is IdentifierNameSyntax identifier
            && StringComparer.Ordinal.Equals(identifier.Identifier.Text, "nameof")
            && invocation.ArgumentList.Arguments.Count == 1;
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

    // How a case builder or [MemberData] provider composes the cases it returns. Recognised: collection expressions,
    // array creations and argument-less collection initialisers (whose elements are read), spreads, Concat, Append,
    // Prepend, ToArray, ToList and Empty from System.Linq.Enumerable, Array.Empty, DispatcherCaseData.ToTheoryData, a
    // static BuildDispatcherCases call (recorded, not read into) and a local initialised with any of these that is
    // read nowhere else; an iterator's "yield return" elements are read the same way. A property provider's getter (or
    // its getter-only initialiser) and a readonly field provider's initialiser are read as a method's return is.
    // Anything else leaves the shape unrecognised, so every check built on it abstains: they are errors that cannot be
    // suppressed, and a guess could fail a build whose cases are complete. An element is read when it is a
    // DispatcherCaseData.Case call, a (name, action) tuple literal or a collection initialiser's { name, action } Add
    // element; any other tuple-typed element leaves the shape unrecognised, except as MayHoldUnseenCase describes.
    private sealed class CaseCompositionShape
    {
        private const string EMPTY_METHOD_NAME = "Empty";

        private const string TO_ARRAY_METHOD_NAME = "ToArray";

        private const string TO_LIST_METHOD_NAME = "ToList";

        private const string CONCAT_METHOD_NAME = "Concat";

        private const string APPEND_METHOD_NAME = "Append";

        private const string PREPEND_METHOD_NAME = "Prepend";

        private readonly CancellationToken _cancellationToken;

        private readonly HashSet<string> _caseNames = new(StringComparer.Ordinal);

        private readonly List<IMethodSymbol> _composedBuilders = [];

        private readonly SyntaxNode _memberSyntax;

        private readonly SemanticModel _semanticModel;

        private readonly WellKnownSymbols _symbols;

        private CaseCompositionShape(
            SyntaxNode memberSyntax,
            SemanticModel semanticModel,
            WellKnownSymbols symbols,
            CancellationToken cancellationToken
        )
        {
            this._memberSyntax = memberSyntax;
            this._semanticModel = semanticModel;
            this._symbols = symbols;
            this._cancellationToken = cancellationToken;
            this.IsRecognised = true;
        }

        public bool IsRecognised { get; private set; }

        // A case whose name is only known at runtime (a non-constant expressionText): the shape is still followed,
        // but a check that needs every name must abstain.
        public bool HasUnknowableCaseName { get; private set; }

        public IEnumerable<string> CaseNames => this._caseNames;

        public IReadOnlyList<IMethodSymbol> ComposedBuilders => this._composedBuilders;

        public bool ListsCase(string testName)
        {
            return this._caseNames.Contains(testName);
        }

        public static CaseCompositionShape Read(
            SyntaxNode memberSyntax,
            SemanticModel semanticModel,
            WellKnownSymbols symbols,
            CancellationToken cancellationToken
        )
        {
            CaseCompositionShape shape = new(
                memberSyntax: memberSyntax,
                semanticModel: semanticModel,
                symbols: symbols,
                cancellationToken: cancellationToken
            );

            bool readsAnything = false;

            foreach (ExpressionSyntax returned in ProvidedExpressions(memberSyntax))
            {
                readsAnything = true;
                shape.ReadSequence(returned);
            }

            bool isIterator = false;

            foreach (ExpressionSyntax yielded in YieldedExpressions(memberSyntax))
            {
                isIterator = true;
                shape.ReadElement(element: yielded, isYielded: true);
            }

            // A yield inside a loop can forward another builder's cases one at a time, so a yielded name only known at
            // runtime may be any of a parent's: FTS004 cannot tell that from a missing parent, so the shape is unknown.
            if (isIterator && shape.HasUnknowableCaseName)
            {
                shape.IsRecognised = false;
            }

            // A property or field that provides nothing readable, or whose value can be replaced, may hold any cases.
            bool providesNothing = !readsAnything && !isIterator;

            if (memberSyntax is not MethodDeclarationSyntax && (providesNothing || MayBeReassigned(memberSyntax)))
            {
                shape.IsRecognised = false;
            }

            return shape;
        }

        // Only what the member itself returns or yields is dispatched; a Case(...) evaluated anywhere else (a discard,
        // or the body of a lambda or local function) never reaches a leaf class's [MemberData].
        private static IEnumerable<SyntaxNode> OwnBodyNodes(SyntaxNode memberSyntax)
        {
            return memberSyntax.DescendantNodes(descendIntoChildren: static node =>
                node is not (AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax)
            );
        }

        // A field's only value is its own initialiser (not that of a local inside it), and a property's is what its
        // getter returns or, for an auto-property, its initialiser.
        private static IEnumerable<ExpressionSyntax> ProvidedExpressions(SyntaxNode memberSyntax)
        {
            return memberSyntax switch
            {
                VariableDeclaratorSyntax declarator => InitialValue(declarator.Initializer),
                PropertyDeclarationSyntax property => ReturnedExpressions(property)
                    .Concat(InitialValue(property.Initializer)),
                _ => ReturnedExpressions(memberSyntax),
            };
        }

        private static IEnumerable<ExpressionSyntax> InitialValue(EqualsValueClauseSyntax? initializer)
        {
            return initializer is null ? [] : [initializer.Value];
        }

        // A non-readonly field or a settable auto-property can be given other cases after its initialiser has run.
        private static bool MayBeReassigned(SyntaxNode memberSyntax)
        {
            return memberSyntax switch
            {
                VariableDeclaratorSyntax { Parent.Parent: FieldDeclarationSyntax field } => !field.Modifiers.Any(
                    SyntaxKind.ReadOnlyKeyword
                ),
                PropertyDeclarationSyntax { Initializer: not null, AccessorList: { } accessorList } =>
                    accessorList.Accessors.Any(static accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration)),
                _ => false,
            };
        }

        private static IEnumerable<ExpressionSyntax> ReturnedExpressions(SyntaxNode memberSyntax)
        {
            return OwnBodyNodes(memberSyntax)
                .Select(static node =>
                    node switch
                    {
                        ReturnStatementSyntax returnStatement => returnStatement.Expression,
                        ArrowExpressionClauseSyntax expressionBody => expressionBody.Expression,
                        _ => null,
                    }
                )
                .OfType<ExpressionSyntax>();
        }

        // Each "yield return" is one element; a provider's yielded object[] rows add no name, leaving its nameof(...)
        // names to speak for it as before.
        private static IEnumerable<ExpressionSyntax> YieldedExpressions(SyntaxNode memberSyntax)
        {
            return OwnBodyNodes(memberSyntax)
                .OfType<YieldStatementSyntax>()
                .Where(static yieldStatement => yieldStatement.IsKind(SyntaxKind.YieldReturnStatement))
                .Select(static yieldStatement => yieldStatement.Expression)
                .OfType<ExpressionSyntax>();
        }

        private void ReadSequence(ExpressionSyntax? sequence)
        {
            switch (sequence)
            {
                case CollectionExpressionSyntax collection:
                    this.ReadCollectionExpression(collection);

                    break;
                case ArrayCreationExpressionSyntax { Initializer: { } initializer }:
                    this.ReadElements(initializer.Expressions);

                    break;
                case ImplicitArrayCreationExpressionSyntax implicitArray:
                    this.ReadElements(implicitArray.Initializer.Expressions);

                    break;
                case BaseObjectCreationExpressionSyntax creation:
                    this.ReadObjectCreation(creation);

                    break;
                case IdentifierNameSyntax identifier:
                    this.ReadLocal(identifier);

                    break;
                case InvocationExpressionSyntax invocation:
                    this.ReadInvocation(invocation);

                    break;
                default:
                    this.IsRecognised = false;

                    break;
            }
        }

        private void ReadCollectionExpression(CollectionExpressionSyntax collection)
        {
            foreach (CollectionElementSyntax element in collection.Elements)
            {
                if (element is SpreadElementSyntax spread)
                {
                    this.ReadSequence(spread.Expression);
                }
                else if (element is ExpressionElementSyntax expressionElement)
                {
                    this.ReadElement(element: expressionElement.Expression, isYielded: false);
                }
            }
        }

        // Constructor arguments (a source collection, say) or an object initialiser could add cases unseen, so only
        // an argument-less constructor with at most a collection initialiser is followed.
        private void ReadObjectCreation(BaseObjectCreationExpressionSyntax creation)
        {
            if (
                creation.ArgumentList is { Arguments.Count: > 0 }
                || creation.Initializer?.IsKind(SyntaxKind.CollectionInitializerExpression) == false
            )
            {
                this.IsRecognised = false;

                return;
            }

            if (creation.Initializer is { } initializer)
            {
                this.ReadElements(initializer.Expressions);
            }
        }

        // A local is followed only when its initialiser is all it ever holds: read once, here, with no other reference
        // (an Add(...), a reassignment, a ref) that could change its cases unseen.
        private void ReadLocal(IdentifierNameSyntax identifier)
        {
            if (
                this._semanticModel.GetSymbolInfo(identifier, this._cancellationToken).Symbol is ILocalSymbol local
                && this.FindDeclarator(local) is { Initializer.Value: { } initialValue }
                && this.IsReferencedOnce(local)
            )
            {
                this.ReadSequence(initialValue);

                return;
            }

            this.IsRecognised = false;
        }

        // Null for a local that is not declared by a plain declarator in this member (a foreach or pattern variable,
        // an out var), which has no initialiser to follow.
        private VariableDeclaratorSyntax? FindDeclarator(ILocalSymbol local)
        {
            return this
                ._memberSyntax.DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault(declarator =>
                    SymbolEqualityComparer.Default.Equals(
                        this._semanticModel.GetDeclaredSymbol(declarator, this._cancellationToken),
                        local
                    )
                );
        }

        private bool IsReferencedOnce(ILocalSymbol local)
        {
            return this._memberSyntax.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(identifier =>
                        StringComparer.Ordinal.Equals(identifier.Identifier.ValueText, local.Name)
                        && SymbolEqualityComparer.Default.Equals(
                            this._semanticModel.GetSymbolInfo(identifier, this._cancellationToken).Symbol,
                            local
                        )
                    )
                    .Take(2)
                    .Count() == 1;
        }

        private void ReadInvocation(InvocationExpressionSyntax invocation)
        {
            if (
                this._semanticModel.GetOperation(invocation, this._cancellationToken)
                is not IInvocationOperation operation
            )
            {
                this.IsRecognised = false;

                return;
            }

            IMethodSymbol method = operation.TargetMethod.ReducedFrom ?? operation.TargetMethod;

            // Recorded by name, not by IsCaseBuilder's shape: composing in something that only looks like a builder
            // (another type's, say) still leaves out the parent's cases, which is what FTS004 reports.
            if (method.IsStatic && StringComparer.Ordinal.Equals(method.Name, BUILDER_METHOD_NAME))
            {
                this._composedBuilders.Add(method);

                return;
            }

            if (!this.IsSequenceOperatorOwner(method.ContainingType))
            {
                this.IsRecognised = false;

                return;
            }

            this.ReadSequenceOperator(operation: operation, methodName: method.Name);
        }

        // Matched by owning type as well as name, so a same-named method anywhere else is not mistaken for one.
        private bool IsSequenceOperatorOwner(INamedTypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Array
                || SymbolEqualityComparer.Default.Equals(type, this._symbols.EnumerableType)
                || SymbolEqualityComparer.Default.Equals(type, this._symbols.DispatcherCaseDataType);
        }

        private void ReadSequenceOperator(IInvocationOperation operation, string methodName)
        {
            switch (methodName)
            {
                case EMPTY_METHOD_NAME:
                    break;
                case TO_ARRAY_METHOD_NAME or TO_LIST_METHOD_NAME or TO_THEORY_DATA_METHOD_NAME:
                    this.ReadSequence(Operand(operation: operation, ordinal: 0));

                    break;
                case CONCAT_METHOD_NAME:
                    this.ReadSequence(Operand(operation: operation, ordinal: 0));
                    this.ReadSequence(Operand(operation: operation, ordinal: 1));

                    break;
                case APPEND_METHOD_NAME or PREPEND_METHOD_NAME:
                    this.ReadSequence(Operand(operation: operation, ordinal: 0));
                    if (Operand(operation: operation, ordinal: 1) is { } element)
                    {
                        this.ReadElement(element: element, isYielded: false);
                    }

                    break;
                default:
                    this.IsRecognised = false;

                    break;
            }
        }

        // By parameter rather than position, so "x.Concat(y)", "Enumerable.Concat(x, y)" and named arguments in any
        // order all read the same operands; an extension method's receiver is its first argument.
        private static ExpressionSyntax? Operand(IInvocationOperation operation, int ordinal)
        {
            SyntaxNode? syntax = operation
                .Arguments.FirstOrDefault(candidate => candidate.Parameter?.Ordinal == ordinal)
                ?.Syntax;

            return syntax is ArgumentSyntax argument ? argument.Expression : syntax as ExpressionSyntax;
        }

        private void ReadElements(IEnumerable<ExpressionSyntax> elements)
        {
            foreach (ExpressionSyntax element in elements)
            {
                this.ReadElement(element: element, isYielded: false);
            }
        }

        private void ReadElement(ExpressionSyntax element, bool isYielded)
        {
            // Checked on syntax: a tuple literal holding a lambda has no natural type for the fallback below to see.
            if (element is TupleExpressionSyntax { Arguments.Count: 2 } tuple)
            {
                this.ReadCaseName(tuple.Arguments[0].Expression);

                return;
            }

            if (
                element is InitializerExpressionSyntax addElement
                && addElement.IsKind(SyntaxKind.ComplexElementInitializerExpression)
            )
            {
                this.ReadAddElement(addElement);

                return;
            }

            if (
                this._semanticModel.GetOperation(element, this._cancellationToken)
                    is IInvocationOperation caseInvocation
                && IsDispatcherCaseDataMethod(
                    method: caseInvocation.TargetMethod,
                    methodName: CASE_METHOD_NAME,
                    symbols: this._symbols
                )
            )
            {
                this.ReadCase(caseInvocation);

                return;
            }

            if (this.MayHoldUnseenCase(element: element, isYielded: isYielded))
            {
                this.IsRecognised = false;
            }
        }

        // A collection initialiser's "{ name, action }" element is an Add(name, action) call, which TheoryData dispatches
        // under its name; an Add of any other arity is not a case this check knows how to name.
        private void ReadAddElement(InitializerExpressionSyntax element)
        {
            if (element.Expressions.Count != 2)
            {
                this.IsRecognised = false;

                return;
            }

            this.ReadCaseName(element.Expressions[0]);
        }

        // ToTheoryData dispatches a (name, action) tuple under its first element verbatim, whatever the action calls;
        // tuple element labels do not reorder it, so the name is always the first argument. An Add element's name is
        // its first argument in the same way.
        private void ReadCaseName(ExpressionSyntax nameExpression)
        {
            Optional<object?> name = this._semanticModel.GetConstantValue(nameExpression, this._cancellationToken);

            if (!name.HasValue)
            {
                this.HasUnknowableCaseName = true;

                return;
            }

            if (name.Value is string caseName)
            {
                this._caseNames.Add(caseName);
            }
        }

        // A tuple-typed variable, member, conditional or cast may hold any case, so it cannot be read. Another type's
        // method returning a tuple is left adding no name in a collection, as before; yielded, it may be forwarding a
        // parent's cases inside a loop, so it too is unknown there. Elements of any other type (a provider's object[]
        // row, say) never were cases.
        private bool MayHoldUnseenCase(ExpressionSyntax element, bool isYielded)
        {
            return (isYielded || element is not InvocationExpressionSyntax)
                && this._semanticModel.GetTypeInfo(element, this._cancellationToken).Type is { IsTupleType: true };
        }

        private void ReadCase(IInvocationOperation caseInvocation)
        {
            if (!TryGetCaseExpressionText(caseInvocation: caseInvocation, expressionText: out string? expressionText))
            {
                this.HasUnknowableCaseName = true;

                return;
            }

            // DispatcherCaseName is the same source DispatcherCaseData.Case parses with at runtime, so the name checked
            // here is exactly the name the case is dispatched under.
            if (
                expressionText is not null
                && DispatcherCaseName.TryGetCaseName(expressionText: expressionText, out string name)
            )
            {
                this._caseNames.Add(name);
            }
        }

        // Resolves the text DispatcherCaseData.Case parses at runtime: an explicitly passed expressionText wins over
        // the CallerArgumentExpression of the action argument, wherever either sits in the argument list. False means
        // the text is only known at runtime; a true result with null text is an explicit null, which never dispatches.
        private static bool TryGetCaseExpressionText(IInvocationOperation caseInvocation, out string? expressionText)
        {
            IArgumentOperation? explicitExpressionText = FindArgument(
                caseInvocation: caseInvocation,
                parameterName: CASE_EXPRESSION_TEXT_PARAMETER_NAME
            );

            if (explicitExpressionText is { ArgumentKind: ArgumentKind.Explicit })
            {
                Optional<object?> constant = explicitExpressionText.Value.ConstantValue;
                expressionText = constant.HasValue ? constant.Value as string : null;

                return constant.HasValue;
            }

            // ArgumentSyntax.Expression drops any "action:" name, matching what CallerArgumentExpression captures.
            expressionText = (
                FindArgument(caseInvocation: caseInvocation, parameterName: CASE_ACTION_PARAMETER_NAME)?.Syntax
                as ArgumentSyntax
            )?.Expression.ToString();

            return true;
        }

        private static IArgumentOperation? FindArgument(IInvocationOperation caseInvocation, string parameterName)
        {
            return caseInvocation.Arguments.FirstOrDefault(argument =>
                StringComparer.Ordinal.Equals(argument.Parameter?.Name, parameterName)
            );
        }

        private static bool IsDispatcherCaseDataMethod(
            IMethodSymbol method,
            string methodName,
            WellKnownSymbols symbols
        )
        {
            return StringComparer.Ordinal.Equals(method.Name, methodName)
                && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbols.DispatcherCaseDataType);
        }
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
            this.DispatcherCaseDataType = compilation.GetTypeByMetadataName("FunFair.Test.Common.DispatcherCaseData");
            this.EnumerableType = compilation.GetTypeByMetadataName("System.Linq.Enumerable");
        }

        public INamedTypeSymbol? DispatchedTestAttribute { get; }

        public INamedTypeSymbol? TheoryAttribute { get; }

        public INamedTypeSymbol? MemberDataAttribute { get; }

        public INamedTypeSymbol? ActionType { get; }

        public INamedTypeSymbol? DispatcherCaseDataType { get; }

        public INamedTypeSymbol? EnumerableType { get; }

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

    // FTS001 looks up a leaf's dispatcher once, but FTS002 does so for every static method declared on the leaf or a
    // type nested in it, so the member and attribute scan is done once per type for the whole compilation. A null
    // value records that the type has no dispatcher.
    private sealed class DispatcherCache
    {
        private ImmutableDictionary<INamedTypeSymbol, IMethodSymbol?> _cache = ImmutableDictionary.Create<
            INamedTypeSymbol,
            IMethodSymbol?
        >(SymbolEqualityComparer.Default);

        public IMethodSymbol? GetOrAdd(INamedTypeSymbol classSymbol, WellKnownSymbols symbols)
        {
            return ImmutableInterlocked.GetOrAdd(
                location: ref this._cache,
                key: classSymbol,
                valueFactory: static (key, s) => FindDispatcher(classSymbol: key, symbols: s),
                factoryArgument: symbols
            );
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

        private static bool IsDispatcherShape(
            IMethodSymbol method,
            INamedTypeSymbol classSymbol,
            WellKnownSymbols symbols
        )
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
    }
}
