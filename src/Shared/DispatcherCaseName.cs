namespace FunFair.Test.Shared;

// Compiled into both FunFair.Test.Common (DispatcherCaseData.Case names each case at runtime) and
// FunFair.Test.Source.Generator (AotTestDispatcherAnalyzer checks the names at build time), so the name the analyzer
// checks can never drift from the name a case is dispatched under. Must stay netstandard2.0-compatible for the
// analyzer, and free of Roslyn for the runtime: no range or index operators, just string scanning. Spans of the text
// are passed as index bounds rather than cut out with Substring, which the runtime target's analyzers would want as ranges.
internal static class DispatcherCaseName
{
    private const int ARROW_LENGTH = 2;

    // The case name is the method the lambda's body calls as its outermost call, the one whose result is the body's
    // value, without any generic type arguments: "t => t.Check(x.Get())" and "t => t.Example<int>()" name the cases
    // "Check" and "Example", and "t => t.X().Y()" names it "Y" because Y is called on X's result. A block holding a
    // single expression statement is read as that expression. False for anything else: no call, an empty or
    // multi-statement block, a body that is not a call, or text that cannot be scanned confidently (an interpolated or
    // raw string literal, or a comment), so a case is never named after a guess.
    public static bool TryGetCaseName(string expressionText, out string name)
    {
        name = string.Empty;

        return TryGetBodyExpression(text: expressionText, out int start, out int end)
            && TryFindOutermostCall(text: expressionText, start: start, end: end, out int openParen)
            && TryReadMethodName(text: expressionText, start: start, openParen: openParen, name: out name);
    }

    // The [start, end) span after the lambda's top-level "=>", which leaves out any static/async modifier and the
    // parameter list, or the whole text when there is no arrow, trimmed of whitespace. "{ <expression>; }" is read as
    // <expression>; any other block, such as "{ }", is rejected. A second statement in the block leaves a top-level
    // ";" in the span, which TryFindOutermostCall then rejects.
    private static bool TryGetBodyExpression(string text, out int start, out int end)
    {
        end = SkipWhitespaceBackwards(text: text, index: text.Length - 1, lowerBound: 0) + 1;
        start = end;

        if (!TryScanTopLevel(text: text, start: 0, end: text.Length, out int arrow, out _, out _))
        {
            return false;
        }

        start = SkipWhitespaceForwards(text: text, index: arrow < 0 ? 0 : arrow + ARROW_LENGTH, end: end);

        if (start == end || text[start] != '{')
        {
            return true;
        }

        int statementStart = SkipWhitespaceForwards(text: text, index: start + 1, end: end - 1);
        int terminator = SkipWhitespaceBackwards(text: text, index: end - 2, lowerBound: statementStart);

        if (terminator < statementStart || text[terminator] != ';')
        {
            return false;
        }

        start = statementStart;
        end = SkipWhitespaceBackwards(text: text, index: terminator - 1, lowerBound: statementStart) + 1;

        return true;
    }

    // The [start, end) span is a call when its last top-level ")" ends it; the last top-level "(" is then that call's,
    // as any later one would sit inside it.
    private static bool TryFindOutermostCall(string text, int start, int end, out int openParen)
    {
        return TryScanTopLevel(text: text, start: start, end: end, out _, out openParen, out int closeParen)
            && openParen >= 0
            && closeParen == end - 1;
    }

    // The identifier before the call's "(", skipping a generic type argument list, provided it is a member access or
    // starts the span; anything else before it (a "new", a parenthesised delegate) is not a method call.
    private static bool TryReadMethodName(string text, int start, int openParen, out string name)
    {
        name = string.Empty;

        int last = SkipWhitespaceBackwards(text: text, index: openParen - 1, lowerBound: start);

        if (last >= start && text[last] == '>')
        {
            last = SkipWhitespaceBackwards(
                text: text,
                index: FindTypeArgumentListStart(text: text, lowerBound: start, close: last) - 1,
                lowerBound: start
            );
        }

        int beforeIdentifier = last;

        while (beforeIdentifier >= start && IsIdentifierCharacter(text[beforeIdentifier]))
        {
            --beforeIdentifier;
        }

        int qualifier = SkipWhitespaceBackwards(text: text, index: beforeIdentifier, lowerBound: start);

        if (beforeIdentifier == last || qualifier >= start && text[qualifier] != '.')
        {
            return false;
        }

        name = text.Substring(startIndex: beforeIdentifier + 1, length: last - beforeIdentifier);

        return true;
    }

    // Walks the [start, end) span tracking "()", "[]" and "{}" depth, skipping string and character literals, and
    // records the first top-level "=>" and the last top-level "(" and ")". False when the brackets do not balance, a
    // literal is unterminated or cannot be skipped confidently, there is a comment, or a top-level ";" separates
    // statements.
    private static bool TryScanTopLevel(
        string text,
        int start,
        int end,
        out int firstArrow,
        out int lastOpenParen,
        out int lastCloseParen
    )
    {
        firstArrow = -1;
        lastOpenParen = -1;
        lastCloseParen = -1;
        int depth = 0;
        int index = start;

        while (index < end)
        {
            char current = text[index];

            if (current is '"' or '\'')
            {
                index = SkipLiteral(text: text, openQuote: index);

                if (index < 0)
                {
                    return false;
                }

                continue;
            }

            if (IsCommentStart(text: text, index: index) || depth == 0 && current == ';')
            {
                return false;
            }

            if (depth == 0 && current == '(')
            {
                lastOpenParen = index;
            }
            else if (depth == 0 && firstArrow < 0 && IsArrow(text: text, index: index))
            {
                firstArrow = index;
            }

            depth += BracketDepthChange(current);

            if (depth < 0)
            {
                return false;
            }

            if (depth == 0 && current == ')')
            {
                lastCloseParen = index;
            }

            ++index;
        }

        return depth == 0;
    }

    // The index just past the literal's closing quote, or -1 when it is unterminated or is an interpolated or raw
    // string, whose holes and delimiters would need a full lexer to skip.
    private static int SkipLiteral(string text, int openQuote)
    {
        char quote = text[openQuote];
        char prefix = openQuote > 0 ? text[openQuote - 1] : ' ';

        if (quote == '"' && IsInterpolatedOrRawString(text: text, openQuote: openQuote, prefix: prefix))
        {
            return -1;
        }

        bool verbatim = prefix == '@';
        int index = openQuote + 1;

        while (index < text.Length)
        {
            char current = text[index];

            if (current == quote)
            {
                if (!verbatim || !IsAt(text: text, index: index + 1, expected: '"'))
                {
                    return index + 1;
                }

                // A verbatim string's doubled quote is an escaped quote, not the end of the string.
                ++index;
            }
            else if (current == '\\' && !verbatim)
            {
                ++index;
            }

            ++index;
        }

        return -1;
    }

    private static bool IsInterpolatedOrRawString(string text, int openQuote, char prefix)
    {
        return prefix == '$'
            || prefix == '@' && openQuote > 1 && text[openQuote - 2] == '$'
            || IsAt(text: text, index: openQuote + 1, expected: '"')
                && IsAt(text: text, index: openQuote + 2, expected: '"');
    }

    // The index of the "<" matching the ">" at close, or -1 when there is none at or after lowerBound.
    private static int FindTypeArgumentListStart(string text, int lowerBound, int close)
    {
        int depth = 0;

        for (int index = close; index >= lowerBound; --index)
        {
            if (text[index] == '>')
            {
                ++depth;
            }
            else if (text[index] == '<')
            {
                --depth;

                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private static int SkipWhitespaceForwards(string text, int index, int end)
    {
        while (index < end && char.IsWhiteSpace(text[index]))
        {
            ++index;
        }

        return index;
    }

    private static int SkipWhitespaceBackwards(string text, int index, int lowerBound)
    {
        while (index >= lowerBound && char.IsWhiteSpace(text[index]))
        {
            --index;
        }

        return index;
    }

    private static int BracketDepthChange(char current)
    {
        return current switch
        {
            '(' or '[' or '{' => 1,
            ')' or ']' or '}' => -1,
            _ => 0,
        };
    }

    private static bool IsCommentStart(string text, int index)
    {
        return text[index] == '/'
            && (IsAt(text: text, index: index + 1, expected: '/') || IsAt(text: text, index: index + 1, expected: '*'));
    }

    private static bool IsArrow(string text, int index)
    {
        return text[index] == '=' && IsAt(text: text, index: index + 1, expected: '>');
    }

    private static bool IsAt(string text, int index, char expected)
    {
        return index < text.Length && text[index] == expected;
    }

    private static bool IsIdentifierCharacter(char current)
    {
        return char.IsLetterOrDigit(current) || current == '_';
    }
}
