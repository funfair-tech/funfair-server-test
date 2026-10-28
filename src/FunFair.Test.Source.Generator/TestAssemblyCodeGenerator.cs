using System;
using System.Diagnostics;
using Credfeto.SourceGeneration.Helpers.Builders;
using FunFair.Test.Source.Generator.Extensions;
using FunFair.Test.Source.Generator.Helpers;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class TestAssemblyCodeGenerator : IIncrementalGenerator
{
    private static readonly NamespaceError IgnoreResult = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(ExtractNamespaces(context), action: GenerateVersionInformation);
    }

    // The generator instance outlives a single compilation in IDE and build-server hosts, so the pipeline must be
    // stateless: each stage yields value-equal data derived only from the current compilation, and generating once
    // per assembly falls out of reducing every namespace declaration to a single flag.
    private static IncrementalValueProvider<(
        NamespaceError Left,
        AnalyzerConfigOptionsProvider Right
    )> ExtractNamespaces(in IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<bool> hasNamespace = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (syntaxNode, _) =>
                    syntaxNode is NamespaceDeclarationSyntax or FileScopedNamespaceDeclarationSyntax,
                transform: static (_, _) => true
            )
            .Collect()
            .Select(static (namespaces, _) => !namespaces.IsEmpty);

        IncrementalValueProvider<string?> assemblyName = context.CompilationProvider.Select(
            static (compilation, _) => compilation.AssemblyName
        );

        return assemblyName
            .Combine(hasNamespace)
            .Select(
                static (item, _) => item.Right ? new NamespaceError(new NamespaceGeneration(item.Left)) : IgnoreResult
            )
            .Combine(context.AnalyzerConfigOptionsProvider);
    }

    private static void ReportException(Location location, in SourceProductionContext context, Exception exception)
    {
        context.ReportDiagnostic(
            diagnostic: Diagnostic.Create(CreateUnhandledExceptionDiagnostic(exception), location: location)
        );
    }

    private static DiagnosticDescriptor CreateUnhandledExceptionDiagnostic(Exception exception)
    {
        return new(
            id: "VER001",
            title: "Unhandled Exception",
            exception.Message + ' ' + exception.StackTrace,
            category: RuntimeVersionInformation.ToolName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }

    private static void GenerateVersionInformation(
        SourceProductionContext sourceProductionContext,
        (NamespaceError Left, AnalyzerConfigOptionsProvider Right) item
    )
    {
        if (item.Right.AreSourceGeneratorsDisabled())
        {
            return;
        }

        if (item.Left.ErrorInfo is not null)
        {
            ErrorInfo ei = item.Left.ErrorInfo.Value;
            ReportException(location: ei.Location, context: sourceProductionContext, exception: ei.Exception);

            return;
        }

        if (item.Left.NamespaceInfo is null)
        {
            return;
        }

        GenerateAssemblySettings(
            sourceProductionContext: sourceProductionContext,
            analyzerConfigOptionsProvider: item.Right
        );
    }

    private static void GenerateAssemblySettings(
        in SourceProductionContext sourceProductionContext,
        AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider
    )
    {
        CodeBuilder source = new CodeBuilder()
            .AppendNamespaces("System.Diagnostics.CodeAnalysis")
            .AppendBlankLine()
            .AppendLine("[assembly: ExcludeFromCodeCoverage]")
            .AppendBlankLine()
            .AppendSuppression(
                category: "Philips.CodeAnalysis.MaintainabilityAnalyzers",
                checkId: "PH2140: Avoid ExcludeFromCodeCoverage",
                justification: "This is a unit test assembly - no need for coverage of the test code itself"
            )
            .AppendSuppression(
                category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer",
                checkId: "PH2071: Duplicate code",
                justification: "This is a unit test assembly - Highly likely there will be duplicate code"
            )
            .AppendSuppression(
                category: "Philips.CodeAnalysis.MaintainabilityAnalyzers",
                checkId: "PH2088: Filename too long",
                justification: "This is a unit test assembly - Highly likely there will be duplicate code"
            )
            .AppendSuppression(
                category: "Microsoft.Performance",
                checkId: "CA1873: Evaluation of this argument may be expensive and unnecessary if logging is disabled",
                justification: "This is a unit test assembly - not so worried about performance"
            )
            .AppendSuppression(
                category: "Microsoft.Performance",
                checkId: "CA2254: Use a fixed template in logging",
                justification: "This is a unit test assembly - Although nice to have a fixed template, its not needed here"
            );

        AppendOptions(options: analyzerConfigOptionsProvider.GlobalOptions, source: source, context: "Global");

        sourceProductionContext.AddSource(hintName: "AssemblySettings.generated.cs", sourceText: source.Text);
    }

    [Conditional("DEBUG")]
    private static void AppendOptions(AnalyzerConfigOptions options, CodeBuilder source, string context)
    {
        foreach (string? thing in options.Keys)
        {
            if (!options.TryGetValue(key: thing, out string? value))
            {
                value = "????????";
            }

            source = source.AppendLine($"// {context}::{thing} = {value}");
        }
    }
}
