using System;
using FunFair.Test.Common;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests.Models;

public sealed class NamespaceErrorTests : TestBase
{
    [Fact]
    public void DefaultConstructor_ExposesNoNamespaceInfoOrErrorInfo()
    {
        NamespaceError result = new();

        Assert.Null(result.NamespaceInfo);
        Assert.Null(result.ErrorInfo);
    }

    [Fact]
    public void Constructor_WithNamespaceGeneration_ExposesNamespaceInfoAndNoErrorInfo()
    {
        NamespaceGeneration namespaceInfo = new(assembly: new(name: "SampleAssembly"));

        NamespaceError result = new(namespaceInfo);

        Assert.Equal(expected: namespaceInfo, actual: result.NamespaceInfo);
        Assert.Null(result.ErrorInfo);
    }

    [Fact]
    public void Constructor_WithErrorInfo_ExposesErrorInfoAndNoNamespaceInfo()
    {
        ErrorInfo errorInfo = new(location: Location.None, exception: new InvalidOperationException("boom"));

        NamespaceError result = new(errorInfo);

        Assert.Equal(expected: errorInfo, actual: result.ErrorInfo);
        Assert.Null(result.NamespaceInfo);
    }
}
