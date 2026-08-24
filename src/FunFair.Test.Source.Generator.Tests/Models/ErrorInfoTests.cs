using System;
using FunFair.Test.Common;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests.Models;

public sealed class ErrorInfoTests : TestBase
{
    [Fact]
    public void Constructor_ExposesLocationAndException()
    {
        Location location = Location.None;
        InvalidOperationException exception = new("boom");

        ErrorInfo errorInfo = new(location: location, exception: exception);

        Assert.Equal(expected: location, actual: errorInfo.Location);
        Assert.Same(expected: exception, actual: errorInfo.Exception);
    }
}
