using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Logging;

[DebuggerDisplay(
    "Include Scopes: {IncludeScopes}, Cat: {IncludeCategory} LogLevel: {IncludeLogLevel}, Timestamp: {TimestampFormat}, UseUtcTimestamp: {UseUtcTimestamp}"
)]
internal readonly record struct XUnitLoggerOptions(
    bool IncludeScopes,
    bool IncludeCategory,
    bool IncludeLogLevel,
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? TimestampFormat,
    bool UseUtcTimestamp
)
{
    public static XUnitLoggerOptions Default { get; } = new(false, false, false, null, true);
};
