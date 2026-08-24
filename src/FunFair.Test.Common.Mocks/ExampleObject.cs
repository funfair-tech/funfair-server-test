using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Mocks;

public sealed class ExampleObject
{
    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedAutoPropertyAccessor.Global",
        Justification = "Used for testing"
    )]
    public string Name { get; init; } = default!;
}
