using FunFair.Test.Common;
using FunFair.Test.Source.Generator.Models;
using Xunit;

namespace FunFair.Test.Source.Generator.Tests.Models;

public sealed class NamespaceGenerationTests : TestBase
{
    [Fact]
    public void Constructor_ExposesAssemblyName()
    {
        NamespaceGeneration generation = new(assemblyName: "SampleAssembly");

        Assert.Equal(expected: "SampleAssembly", actual: generation.AssemblyName);
    }
}
