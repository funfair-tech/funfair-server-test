using FunFair.Test.Common;
using FunFair.Test.Source.Generator.Models;
using Microsoft.CodeAnalysis;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests.Models;

public sealed class NamespaceGenerationTests : TestBase
{
    [Fact]
    public void Namespace_ReturnsAssemblyName()
    {
        AssemblyIdentity assembly = new(name: "SampleAssembly");

        NamespaceGeneration generation = new(assembly: assembly);

        Assert.Equal(expected: "SampleAssembly", actual: generation.Namespace);
    }

    [Fact]
    public void Assembly_ReturnsConstructorValue()
    {
        AssemblyIdentity assembly = new(name: "SampleAssembly");

        NamespaceGeneration generation = new(assembly: assembly);

        Assert.Equal(expected: assembly, actual: generation.Assembly);
    }
}
