using FunFair.Test.Common.Tests.Mocks.Converters.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure)
    {
    }

    private static void Configure(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(GetSubstitute<ITest>());
        serviceCollection.AddSingleton<IModelBinder, ModelBinder>();
    }

    [Fact]
    public void ModelBinderIsRegistered()
    {
        this.RequireService<IModelBinder>();
    }

    [Fact]
    public void TestIsRegistered()
    {
        this.RequireService<ITest>();
    }
}