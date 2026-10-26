using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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

        // Checked from the provider's own declaration rather than the leaf's symbol action: only a syntax node action
        // has a SemanticModel to resolve each Case(...) through, and Compilation.GetSemanticModel is off-limits in an
        // analyzer (RS1030).
        context.RegisterSyntaxNodeAction(
            nodeContext =>
                AnalyzeDispatcherCaseProvider(
                    nodeContext,
                    symbols: symbols,
                    dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
                ),
            SyntaxKind.MethodDeclaration
        );
    }

    private static void AnalyzeClass(
        in SymbolAnalysisContext context,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        if (context.Symbol is not INamedTypeSymbol classSymbol || !IsLeafClass(classSymbol))
        {
            return;
        }

        INamedTypeSymbol? affectedBaseType = CollectBaseChain(classSymbol)
            .FirstOrDefault(bt => !dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols).IsEmpty);

        if (affectedBaseType is null)
        {
            return;
        }

        if (FindDispatcher(classSymbol: classSymbol, symbols: symbols) is null)
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
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        // [MemberData] only binds to static members, so the cheap syntactic check filters out almost every method.
        if (
            context.Node is not MethodDeclarationSyntax providerSyntax
            || !providerSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
            || context.SemanticModel.GetDeclaredSymbol(providerSyntax, context.CancellationToken)
                is not { ContainingType: { } providerType } providerMethod
        )
        {
            return;
        }

        // A local provider is declared on the leaf itself or on a type nested directly inside it.
        ReportIncompleteDispatcherCases(
            context,
            classSymbol: providerType,
            providerMethod: providerMethod,
            providerSyntax: providerSyntax,
            symbols: symbols,
            dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
        );

        if (providerType.ContainingType is { } outerType)
        {
            ReportIncompleteDispatcherCases(
                context,
                classSymbol: outerType,
                providerMethod: providerMethod,
                providerSyntax: providerSyntax,
                symbols: symbols,
                dispatchedTestNamesByBaseType: dispatchedTestNamesByBaseType
            );
        }
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

        HashSet<string>? caseNames = CollectCaseNames(
            methodSyntax: builderSyntax,
            semanticModel: context.SemanticModel,
            symbols: symbols,
            cancellationToken: context.CancellationToken
        );

        if (caseNames is null)
        {
            return;
        }

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

        SemanticModel semanticModel = context.SemanticModel;
        CancellationToken cancellationToken = context.CancellationToken;

        bool spreadsParentBuilder = builderSyntax
            .DescendantNodes()
            .OfType<SpreadElementSyntax>()
            .Any(spread =>
                IsCaseBuilderOf(
                    expression: spread.Expression,
                    builderOwner: parentWithBuilder,
                    semanticModel: semanticModel,
                    cancellationToken: cancellationToken
                )
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

    // Resolved rather than matched by name, so a grandparent's, an unrelated type's or the builder's own
    // BuildDispatcherCases (a copy-paste slip that drops the parent's cases or recurses forever) is not mistaken for
    // the parent's.
    private static bool IsCaseBuilderOf(
        ExpressionSyntax expression,
        INamedTypeSymbol builderOwner,
        SemanticModel semanticModel,
        CancellationToken cancellationToken
    )
    {
        return semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol is IMethodSymbol method
            && StringComparer.Ordinal.Equals(method.Name, BUILDER_METHOD_NAME)
            && SymbolEqualityComparer.Default.Equals(
                method.ContainingType.OriginalDefinition,
                builderOwner.OriginalDefinition
            );
    }

    // Null means a case's name cannot be known at compile time (a non-constant expressionText), so the caller abstains:
    // these diagnostics are errors that cannot be suppressed, so guessing would risk failing a build whose cases are
    // in fact complete.
    private static HashSet<string>? CollectCaseNames(
        MethodDeclarationSyntax methodSyntax,
        SemanticModel semanticModel,
        WellKnownSymbols symbols,
        CancellationToken cancellationToken
    )
    {
        HashSet<string> names = new(StringComparer.Ordinal);

        foreach (
            InvocationExpressionSyntax invocation in ReturnedCollectionElements(
                    methodSyntax: methodSyntax,
                    semanticModel: semanticModel,
                    symbols: symbols,
                    cancellationToken: cancellationToken
                )
                .OfType<InvocationExpressionSyntax>()
        )
        {
            if (
                semanticModel.GetOperation(invocation, cancellationToken) is not IInvocationOperation caseInvocation
                || !IsDispatcherCaseDataMethod(
                    method: caseInvocation.TargetMethod,
                    methodName: CASE_METHOD_NAME,
                    symbols: symbols
                )
            )
            {
                continue;
            }

            if (!TryGetCaseExpressionText(caseInvocation: caseInvocation, expressionText: out string? expressionText))
            {
                return null;
            }

            string? name = expressionText is null ? null : ExtractCaseName(expressionText);

            if (name is not null)
            {
                names.Add(name);
            }
        }

        return names;
    }

    // Resolves the text DispatcherCaseData.Case parses at runtime: an explicitly passed expressionText wins over the
    // CallerArgumentExpression of the action argument, wherever either sits in the argument list. False means the
    // text is only known at runtime; a true result with null text is an explicit null, which never dispatches.
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

    // Only the elements of the collection the method itself returns are dispatched; a Case(...) evaluated anywhere
    // else (a discard, or the body of a lambda or local function) never reaches a leaf class's [MemberData].
    // Yields each element's expression, or the SpreadElementSyntax itself for a spread.
    private static IEnumerable<SyntaxNode> ReturnedCollectionElements(
        MethodDeclarationSyntax methodSyntax,
        SemanticModel semanticModel,
        WellKnownSymbols symbols,
        CancellationToken cancellationToken
    )
    {
        return methodSyntax
            .DescendantNodes(descendIntoChildren: static node =>
                node is not (AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax)
            )
            .Select(static node =>
                node switch
                {
                    ReturnStatementSyntax returnStatement => returnStatement.Expression,
                    ArrowExpressionClauseSyntax expressionBody => expressionBody.Expression,
                    _ => null,
                }
            )
            .OfType<ExpressionSyntax>()
            .Select(returned =>
                UnwrapToTheoryData(
                    returned: returned,
                    semanticModel: semanticModel,
                    symbols: symbols,
                    cancellationToken: cancellationToken
                )
            )
            .SelectMany(CollectionElements);
    }

    // A [MemberData] provider converts its case collection with "<collection>.ToTheoryData()", so the cases are the
    // receiver's elements.
    private static ExpressionSyntax UnwrapToTheoryData(
        ExpressionSyntax returned,
        SemanticModel semanticModel,
        WellKnownSymbols symbols,
        CancellationToken cancellationToken
    )
    {
        return
            returned is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess } invocation
            && semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol method
            && IsDispatcherCaseDataMethod(
                method: method.ReducedFrom ?? method,
                methodName: TO_THEORY_DATA_METHOD_NAME,
                symbols: symbols
            )
            ? memberAccess.Expression
            : returned;
    }

    private static IEnumerable<SyntaxNode> CollectionElements(ExpressionSyntax collection)
    {
        return collection switch
        {
            CollectionExpressionSyntax collectionExpression => collectionExpression.Elements.Select(
                static SyntaxNode (element) =>
                    element is ExpressionElementSyntax expressionElement ? expressionElement.Expression : element
            ),
            ArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
            ImplicitArrayCreationExpressionSyntax implicitArray => implicitArray.Initializer.Expressions,
            _ => [],
        };
    }

    private static bool IsDispatcherCaseDataMethod(IMethodSymbol method, string methodName, WellKnownSymbols symbols)
    {
        return StringComparer.Ordinal.Equals(method.Name, methodName)
            && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbols.DispatcherCaseDataType);
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
        in SyntaxNodeAnalysisContext context,
        INamedTypeSymbol classSymbol,
        IMethodSymbol providerMethod,
        MethodDeclarationSyntax providerSyntax,
        WellKnownSymbols symbols,
        DispatchedTestNameCache dispatchedTestNamesByBaseType
    )
    {
        if (!IsLeafClass(classSymbol))
        {
            return;
        }

        IMethodSymbol? dispatcher = FindDispatcher(classSymbol: classSymbol, symbols: symbols);

        if (
            dispatcher is null
            || !IsProviderOf(
                dispatcher: dispatcher,
                classSymbol: classSymbol,
                providerMethod: providerMethod,
                symbols: symbols
            )
        )
        {
            return;
        }

        HashSet<string>? declaredCaseNames = TryGetDeclaredCaseNames(
            providerSyntax: providerSyntax,
            semanticModel: context.SemanticModel,
            symbols: symbols,
            cancellationToken: context.CancellationToken
        );

        if (declaredCaseNames is null)
        {
            return;
        }

        foreach (
            string testName in CollectBaseChain(classSymbol)
                .SelectMany(bt => dispatchedTestNamesByBaseType.GetOrAdd(baseType: bt, symbols: symbols))
                .Distinct(StringComparer.Ordinal)
                .Where(testName => !declaredCaseNames.Contains(testName))
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

    // The caller only passes the leaf itself or a type nested directly inside it as the provider's owner, so matching
    // the [MemberData] target to this provider also establishes that the provider is local to the leaf.
    private static bool IsProviderOf(
        IMethodSymbol dispatcher,
        INamedTypeSymbol classSymbol,
        IMethodSymbol providerMethod,
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

        return SymbolEqualityComparer.Default.Equals(
            providerType.GetMembers(memberName).OfType<IMethodSymbol>().FirstOrDefault(),
            providerMethod
        );
    }

    // Null means the provider's cases cannot be listed from its own source, so the check abstains: a provider that
    // delegates with "BuildDispatcherCases<TSelf>().ToTheoryData()" (or spreads it) gets its cases from a base
    // builder, whose completeness FTS003 checks where that base is declared.
    private static HashSet<string>? TryGetDeclaredCaseNames(
        MethodDeclarationSyntax providerSyntax,
        SemanticModel semanticModel,
        WellKnownSymbols symbols,
        CancellationToken cancellationToken
    )
    {
        bool spreadsOtherCases = ReturnedCollectionElements(
                methodSyntax: providerSyntax,
                semanticModel: semanticModel,
                symbols: symbols,
                cancellationToken: cancellationToken
            )
            .OfType<SpreadElementSyntax>()
            .Any();

        if (spreadsOtherCases)
        {
            return null;
        }

        HashSet<string>? names = CollectCaseNames(
            methodSyntax: providerSyntax,
            semanticModel: semanticModel,
            symbols: symbols,
            cancellationToken: cancellationToken
        );

        if (names is null)
        {
            return null;
        }

        names.UnionWith(CollectNameofTargets(providerSyntax));

        return names.Count == 0 ? null : names;
    }

    private static IEnumerable<string> CollectNameofTargets(MethodDeclarationSyntax providerSyntax)
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
        }

        public INamedTypeSymbol? DispatchedTestAttribute { get; }

        public INamedTypeSymbol? TheoryAttribute { get; }

        public INamedTypeSymbol? MemberDataAttribute { get; }

        public INamedTypeSymbol? ActionType { get; }

        public INamedTypeSymbol? DispatcherCaseDataType { get; }

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
