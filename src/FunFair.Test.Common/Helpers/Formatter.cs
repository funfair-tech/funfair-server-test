using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Common.Helpers;

public static class Formatter
{
    [SuppressMessage(
        category: "ToStringWithoutOverrideAnalyzer",
        checkId: "ExplicitToStringWithoutOverrideAnalyzer: Calling ToString() on object of type 'T' but it does not override ToString()",
        Justification = "Valid in this case"
    )]
    public static string FormatValue<T>(this T value)
        where T : notnull
    {
        string rv = value.ToString() ?? string.Empty;

        Assert.False(
            StringComparer.Ordinal.Equals(x: rv, y: value.GetType().FullName),
            userMessage: "ToString() not implemented"
        );

        return rv;
    }
}
