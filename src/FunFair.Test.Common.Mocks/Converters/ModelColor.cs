using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Mocks.Converters;

public enum ModelColor
{
    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedMember.Global",
        Justification = "Deliberate for usage in tests"
    )]
    RED,

    BLUE,
}
