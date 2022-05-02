using System;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class SqlTypeMapperTestsBaseTests : SqlTypeMapperTestsBase<ExampleRecordTypeMapper, ExampleRecord>
{
    [Fact]
    public void ShouldParseTest()
    {
        this.ShouldParse(value: "Hello", new("Hello"));
    }

    [Fact]
    public void ShouldNotParseTest()
    {
        this.ShouldNotParse<InvalidCastException, decimal>(value: 1.234m);
    }
}