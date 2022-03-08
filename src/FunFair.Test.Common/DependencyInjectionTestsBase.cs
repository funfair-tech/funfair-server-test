using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

/// <summary>
///     Checks for Dependency Injection issues.
/// </summary>
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
public abstract class DependencyInjectionTestsBase : IntegrationTestBase
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">XUnit output</param>
    /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
    protected DependencyInjectionTestsBase(ITestOutputHelper output, Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration)
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration)
    {
    }

    /// <summary>
    ///     Require that the service is registered.
    /// </summary>
    /// <typeparam name="TService">The service that must be registered</typeparam>
    /// <remarks>This version should be used if there are no async or observables registered in the constructor.</remarks>
    protected void RequireService<TService>()
        where TService : class
    {
        this.RequireServiceCommon<TService>();
    }

    /// <summary>
    ///     Require that the service is registered.
    /// </summary>
    /// <typeparam name="TService">The service that must be registered</typeparam>
    /// <remarks>This version should be used if there is any async or observables registered in the constructor.</remarks>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used in implementations")]
    protected async Task RequireServiceAsync<TService>()
        where TService : class
    {
        TService service = this.RequireServiceCommon<TService>();

        await Task.CompletedTask;

        this.Logger.LogDebug($"Waiting for dispose of {service.GetType().FullName}...");
    }

    /// <summary>
    ///     Require that a service of the named type is registered against an interface, where there may be more than once instance of the interface registration.
    /// </summary>
    /// <typeparam name="TInterface">The interface that the service should be registered for.</typeparam>
    /// <typeparam name="TService">The service that must be registered</typeparam>
    /// <remarks>This version should be used if there are no async or observables registered in the constructor.</remarks>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used in implementations")]
    protected void RequireServiceInCollectionFor<TInterface, TService>()
        where TInterface : class where TService : class, TInterface
    {
        this.RequireServiceInCollectionForCommon<TInterface, TService>();
    }

    /// <summary>
    ///     Require that a service of the named type is registered against an interface, where there may be more than once instance of the interface registration.
    /// </summary>
    /// <typeparam name="TInterface">The interface that the service should be registered for.</typeparam>
    /// <typeparam name="TService">The service that must be registered</typeparam>
    /// <remarks>This version should be used if there is any async or observables registered in the constructor.</remarks>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used in implementations")]
    protected async Task RequireServiceInCollectionForAsync<TInterface, TService>()
        where TInterface : class where TService : class, TInterface
    {
        this.RequireServiceInCollectionForCommon<TInterface, TService>();

        await Task.CompletedTask;

        this.Logger.LogDebug($"Waiting for dispose of {typeof(TInterface).FullName}...");
    }

    private void RequireServiceInCollectionForCommon<TInterface, TService>()
        where TInterface : class where TService : class, TInterface
    {
        IReadOnlyList<TInterface> services = this.ServiceProvider.GetServices<TInterface>()
                                                 .ToArray();

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

    private TService RequireServiceCommon<TService>()
        where TService : class
    {
        TService service = this.GetService<TService>();
        Assert.NotNull(service);

        string fullName = service.GetType()
                                 .FullName ?? string.Empty;
        this.Output.WriteLine($"Type Name: {typeof(TService).FullName}");
        this.Output.WriteLine($"Type Name: {fullName}");

        Assert.False(IsProxyObject(fullName), $"{typeof(TService).FullName} must not be a proxy object - found: {fullName}");

        IReadOnlyList<TService> services = this.ServiceProvider.GetServices<TService>()
                                               .ToArray();

        this.Output.WriteLine("Found Services:");

        foreach (TService foundService in services)
        {
            this.Output.WriteLine($"* {foundService.GetType().FullName}");
        }

        Assert.Single(services);

        return service;
    }

    private static bool IsProxyObject(string fullTypeName)
    {
        return StringComparer.Ordinal.Equals(x: fullTypeName, y: "Castle.Proxies.ObjectProxy") ||
               fullTypeName.StartsWith(value: "Castle.Proxies.ObjectProxy_", comparisonType: StringComparison.Ordinal);
    }
}