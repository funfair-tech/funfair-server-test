using System;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class SqlTypeMapperTestsBaseTests
    : SqlTypeMapperTestsBase<ExampleRecordTypeMapper, ExampleRecord>
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

    [Fact]
    public void ShouldSetValueTest()
    {
        this.ShouldSetValue(new("Test Value"), expected: "Test Value");
    }

    [Fact]
    public void ShouldSetValueBinaryTest()
    {
        this.ShouldSetValue(new("Binary"), "Binary"u8.ToArray());
    }

    [Fact]
    public void ShouldNotSetValueTest()
    {
        this.ShouldNotSetValue<ArgumentOutOfRangeException>(new("Exception"));
    }
}
