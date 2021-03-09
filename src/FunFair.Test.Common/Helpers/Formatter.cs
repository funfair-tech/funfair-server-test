using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Common.Helpers
{
    /// <summary>
    ///     Formats values.
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        ///     Formats the value, and checks that it isn't the name
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>The formatted value.</returns>

        [SuppressMessage(category: "ToStringWithoutOverrideAnalyzer",
                         checkId: "ExplicitToStringWithoutOverrideAnalyzer: Calling ToString() on object of type 'T' but it does not override ToString()",
                         Justification = "TODO: Review")]
        public static string FormatValue<T>(this T value)
            where T : notnull
        {
            string rv = value.ToString() ?? string.Empty;

            Assert.False(rv == value.GetType()
                                    .FullName,
                         userMessage: "ToString() not implemented");

            return rv;
        }
    }
}


