using FunFair.Test.Common.Mocks;

namespace FunFair.Test.Common.Tests.Mocks;

internal static class MockReferenceData
{
    public static readonly MockBase<ExampleObject> ExampleObject = new MockExampleObject();
}