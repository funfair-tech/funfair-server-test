namespace FunFair.Test.Common.Tests.Mocks
{
    internal sealed class MockGenericModel<T>
    {
        public T Value { get; }

        public T[] NestedValue { get; set; }

        public MockGenericModel(T value)
        {
            this.Value = value;
            this.NestedValue = new[] { value };
        }
    }
}