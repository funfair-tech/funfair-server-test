using System;
using FunFair.Test.Common.Tests.Mocks;
using FunFair.Test.Common.Tests.Mocks.Converters.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure) { }

    private static IServiceCollection Configure(IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddMockedService<ITestInterface>()
            .AddMockedService<ITestInterface2>(NoChanges)
            .AddSingleton<IModelBinder, ModelBinder>();
    }

    private static void NoChanges(ITestInterface2 item)
    {
        // Used to force compiler to use the different overload
    }

    [Fact]
    public void ModelBinderIsRegistered()
    {
        this.RequireService<IModelBinder>();
    }

    [Fact]
    public void TestIsRegisteredAsCastleCoreProxy()
    {
        ITestInterface test = this.GetService<ITestInterface>();

        string fullName = test.GetType().FullName ?? string.Empty;
        this.Output.WriteLine(fullName);

        Assert.True(IsProxyObject(fullName), userMessage: "Should be proxy object");
    }

    [Fact]
    public void Test2IsRegisteredAsCastleCoreProxy()
    {
        ITestInterface2 test = this.GetService<ITestInterface2>();

        string fullName = test.GetType().FullName ?? string.Empty;
        this.Output.WriteLine(fullName);

        Assert.True(IsProxyObject(fullName), userMessage: "Should be proxy object");
    }

    [Fact]
    public void RealIsNotRegisteredAsCastleCoreProxy()
    {
        IModelBinder test = this.GetService<IModelBinder>();

        string fullName = test.GetType().FullName ?? string.Empty;
        this.Output.WriteLine(fullName);

        Assert.False(IsProxyObject(fullName), userMessage: "Should not be proxy object");
    }

    private static bool IsProxyObject(string fullTypeName)
    {
        return StringComparer.Ordinal.Equals(x: fullTypeName, y: "Castle.Proxies.ObjectProxy")
            || fullTypeName.StartsWith(
                value: "Castle.Proxies.ObjectProxy_",
                comparisonType: StringComparison.Ordinal
            );
    }
}
