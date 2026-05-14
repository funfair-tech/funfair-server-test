using System;
using FunFair.Test.Common.Mocks;

namespace FunFair.Test.Common.Tests.Mocks;

internal static class MockExampleObject
{
    public static MockBase<ExampleObject> Create()
    {
        return new(new() { Name = "Test" }, () => new() { Name = Guid.NewGuid().ToString() });
    }
}
