using FunFair.Test.Infrastructure.Mocks;

namespace FunFair.Test.Infrastructure.Tests.Mocks;

internal static class MockReferenceData
{
    public static readonly MockBase<ExampleObject> ExampleObject = new MockExampleObject();
}
