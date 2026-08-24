using System;
using FunFair.Test.Infrastructure.Mocks;

namespace FunFair.Test.Common.Mocks;

internal sealed class MockExampleObject : MockBase<ExampleObject>
{
    public MockExampleObject()
        : base(new() { Name = "Test" }) { }

    public override ExampleObject Next()
    {
        return new() { Name = Guid.NewGuid().ToString() };
    }
}
