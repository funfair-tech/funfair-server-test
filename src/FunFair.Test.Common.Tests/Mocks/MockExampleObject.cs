using System;
using FunFair.Test.Common.Mocks;

namespace FunFair.Test.Common.Tests.Mocks;

internal sealed class MockExampleObject : MockBase<ExampleObject>
{
    public MockExampleObject()
        : base(new() { Name = "Test" }) { }

    public override ExampleObject Next()
    {
        return new() { Name = Guid.NewGuid().ToString() };
    }
}
