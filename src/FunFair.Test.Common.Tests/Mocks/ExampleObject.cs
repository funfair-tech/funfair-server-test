using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Tests.Mocks;

internal sealed class ExampleObject
{
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Used for testing")]
    public string Name { get; init; } = default!;
}