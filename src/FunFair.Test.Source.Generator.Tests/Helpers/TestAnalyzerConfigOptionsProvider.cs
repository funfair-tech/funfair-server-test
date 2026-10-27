using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator.Tests.Helpers;

internal sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions)
    {
        this.GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return this.GlobalOptions;
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        return this.GlobalOptions;
    }
}
