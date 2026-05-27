using System.Collections.Immutable;
using System.Linq;
using FunFair.Test.Source.Generator.Builders;
using FunFair.Test.Source.Generator.Extensions;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunFair.Test.Source.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class ProtectedMemberSuppressionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<ProtectedMemberInfo>> allMembers = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    ClassDeclarationSyntax classDecl = (ClassDeclarationSyntax)ctx.Node;

                    if (classDecl.Modifiers.Any(SyntaxKind.SealedKeyword))
                    {
                        return [];
                    }

                    if (ctx.SemanticModel.GetDeclaredSymbol(classDecl, ct) is not INamedTypeSymbol typeSymbol)
                    {
                        return [];
                    }

                    return CollectProtectedMembers(typeSymbol);
                }
            )
            .Where(static items => !items.IsEmpty)
            .Collect()
            .Select(static (allArrays, _) => allArrays.SelectMany(static arr => arr).Distinct().ToImmutableArray());

        context.RegisterSourceOutput(allMembers, GenerateSuppressions);
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
