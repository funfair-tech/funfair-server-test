using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Credfeto.SourceGeneration.Helpers.Builders;
using FunFair.Test.Source.Generator.Extensions;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunFair.Test.Source.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class ProtectedMemberSuppressionGenerator : IIncrementalGenerator
{
    public const string CollectProtectedMembersStep = "CollectProtectedMembers";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<bool> sourceGeneratorsDisabled = context.AnalyzerConfigOptionsProvider.Select(
            static (provider, _) => provider.AreSourceGeneratorsDisabled()
        );

        // Roslyn re-runs a syntax provider's transform on every compilation, so only value-equal results may leave it:
        // a SemanticModel or symbol would root old compilations and never compare equal. The transform's array compares
        // by reference, so it is flattened into value-equal records at once; when those match the previous run, the
        // collect, the opt-out gate and the source output are all reused.
        IncrementalValueProvider<ImmutableArray<ProtectedMemberInfo>> allMembers = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax classDecl && !classDecl.Modifiers.Any(SyntaxKind.SealedKeyword),
                transform: static (ctx, ct) =>
                    CollectProtectedMembers((ClassDeclarationSyntax)ctx.Node, ctx.SemanticModel, ct)
            )
            .SelectMany(static (members, _) => members)
            .WithTrackingName(CollectProtectedMembersStep)
            .Collect()
            .Combine(sourceGeneratorsDisabled)
            .Select(static (item, _) => item.Right ? [] : item.Left.Distinct().ToImmutableArray());

        context.RegisterSourceOutput(allMembers, GenerateSuppressions);
    }

    private static ImmutableArray<ProtectedMemberInfo> CollectProtectedMembers(
        ClassDeclarationSyntax classDecl,
        SemanticModel semanticModel,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return semanticModel.GetDeclaredSymbol(classDecl, cancellationToken) is { } typeSymbol
            ? CollectProtectedMembers(typeSymbol)
            : [];
    }

    private static ImmutableArray<ProtectedMemberInfo> CollectProtectedMembers(INamedTypeSymbol typeSymbol)
    {
        ImmutableArray<ProtectedMemberInfo>.Builder members = ImmutableArray.CreateBuilder<ProtectedMemberInfo>();

        foreach (ISymbol member in typeSymbol.GetMembers())
        {
            if (
                member.DeclaredAccessibility
                is not Accessibility.Protected
                    and not Accessibility.ProtectedOrInternal
                    and not Accessibility.ProtectedAndInternal
            )
            {
                continue;
            }

            if (member.IsImplicitlyDeclared)
            {
                continue;
            }

            string? docId = member.GetDocumentationCommentId();

            if (docId is null)
            {
                continue;
            }

            members.Add(new ProtectedMemberInfo(target: docId));
        }

        return members.ToImmutable();
    }

    private static void GenerateSuppressions(SourceProductionContext ctx, ImmutableArray<ProtectedMemberInfo> members)
    {
        if (members.IsEmpty)
        {
            return;
        }

        CodeBuilder source = new CodeBuilder().AppendNamespaces("System.Diagnostics.CodeAnalysis").AppendBlankLine();

        foreach (ProtectedMemberInfo m in members)
        {
            source = source.AppendLine(
                $"[assembly: SuppressMessage(category: \"ReSharper\", checkId: \"UnusedMember.Global\", Justification = \"May be used in derived test classes\", Scope = \"member\", Target = \"~{m.Target}\")]"
            );
        }

        ctx.AddSource(hintName: "ProtectedMemberSuppressions.generated.cs", sourceText: source.Text);
    }
}
