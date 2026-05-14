using System;
using Xunit;

namespace FunFair.Test.Common.Helpers;

public static class Formatter
{
    public static string FormatValue<T>(this T value)
        where T : notnull
    {
        string rv = string.Concat(value);

        Assert.False(
            StringComparer.Ordinal.Equals(x: rv, y: value.GetType().FullName),
            userMessage: "ToString() not implemented"
        );

        return rv;
    }
}
