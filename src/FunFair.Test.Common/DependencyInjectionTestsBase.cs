using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Test.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FunFair.Test.Common;

public abstract class DependencyInjectionTestsBase : IntegrationTestBase
{
    protected DependencyInjectionTestsBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration
    )
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration) { }

    protected void RequireService<TService>()
        where TService : class
    {
        TService service = this.RequireServiceCommon<TService>();
        UnusedVariable(service);
    }

    protected async Task RequireServiceAsync<TService>()
        where TService : class
    {
        TService service = this.RequireServiceCommon<TService>();

        await Task.CompletedTask;

        this.Logger.LogWaitingForDispose(service.GetType());
    }

    protected void RequireServiceInCollectionFor<TInterface, TService>()
        where TInterface : class
        where TService : class, TInterface
    {
        this.RequireServiceInCollectionForCommon<TInterface, TService>();
    }

    protected async Task RequireServiceInCollectionForAsync<TInterface, TService>()
        where TInterface : class
        where TService : class, TInterface
    {
        this.RequireServiceInCollectionForCommon<TInterface, TService>();

        await Task.CompletedTask;

        this.Logger.LogWaitingForDispose(typeof(TInterface));
    }

    private void RequireServiceInCollectionForCommon<TInterface, TService>()
        where TInterface : class
        where TService : class, TInterface
    {
        IReadOnlyList<TInterface> services = this.GetServices<TInterface>();

        this.DumpServices(services);

        Assert.NotEmpty(services);
        Assert.Contains(collection: services, filter: service => service.GetType() == typeof(TService));
    }

    private void DumpServices<TInterface>(IReadOnlyList<TInterface> services)
        where TInterface : class
    {
        this.Output.WriteLine("Found Services:");

        foreach (TInterface service in services)
        {
            this.Output.WriteLine($"* {service.GetType().FullName}");
        }
    }

    private TInterface RequireServiceCommon<TInterface>()
        where TInterface : class
    {
        TInterface service = this.GetService<TInterface>();
        Assert.NotNull(service);

        string fullName = service.GetType().FullName ?? string.Empty;
        this.Output.WriteLine($"Type Name: {typeof(TInterface).FullName}");
        this.Output.WriteLine($"Type Name: {fullName}");

        Assert.False(
            IsProxyObject(fullName),
            $"{typeof(TInterface).FullName} must not be a proxy object - found: {fullName}"
        );

        IReadOnlyList<TInterface> services = this.GetServices<TInterface>();

        this.Output.WriteLine("Found Services:");

        foreach (TInterface foundService in services)
        {
            this.Output.WriteLine($"* {foundService.GetType().FullName}");
        }

        return Assert.Single(services);
    }

    private IReadOnlyList<TInterface> GetServices<TInterface>()
        where TInterface : class
    {
        return [.. this.ServiceProvider.GetServices<TInterface>()];
    }

    private static bool IsProxyObject(string fullTypeName)
    {
        return StringComparer.Ordinal.Equals(x: fullTypeName, y: "Castle.Proxies.ObjectProxy")
            || fullTypeName.StartsWith(value: "Castle.Proxies.ObjectProxy_", comparisonType: StringComparison.Ordinal);
    }
}
