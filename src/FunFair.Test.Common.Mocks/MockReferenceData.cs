using FunFair.Test.Infrastructure.Mocks;

namespace FunFair.Test.Common.Mocks;

public static class MockReferenceData
{
    public static readonly MockBase<ExampleObject> ExampleObject = new MockExampleObject();
}
