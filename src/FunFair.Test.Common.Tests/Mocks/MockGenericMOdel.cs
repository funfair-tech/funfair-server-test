using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Tests.Mocks;

internal sealed class MockGenericModel<T>
{
    public MockGenericModel(T value)
    {
        this.Value = value;
        this.NestedValue = new[] { value };
    }

    public T Value { get; }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "TODO: Review")]
    public T[] NestedValue { get; set; }
}