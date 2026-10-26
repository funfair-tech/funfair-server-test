namespace FunFair.Test.Shared;

// Compiled into both FunFair.Test.Common (DispatcherCaseData.Case names each case at runtime) and
// FunFair.Test.Source.Generator (AotTestDispatcherAnalyzer checks the names at build time), so the name the analyzer
// checks can never drift from the name a case is dispatched under. Must stay netstandard2.0-compatible for the
// analyzer: no range or index operators.
internal static class DispatcherCaseName
{
    // The case name is the identifier immediately before the last call in the lambda's source text, e.g.
    // "t => t.X().Y()" names the case "Y". False when the text contains no call at all.
    public static bool TryGetCaseName(string expressionText, out string name)
    {
        int openParen = expressionText.LastIndexOf('(');

        if (openParen < 0)
        {
            name = string.Empty;

            return false;
        }

        int dot = expressionText.LastIndexOf(value: '.', startIndex: openParen);

        name = expressionText.Substring(startIndex: dot + 1, length: openParen - dot - 1);

        return true;
    }
}
