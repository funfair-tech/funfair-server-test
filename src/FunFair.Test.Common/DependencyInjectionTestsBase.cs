using System;
using System.Diagnostics.CodeAnalysis;
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
    protected DependencyInjectionTestsBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration)
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration)
    {
    }

    /// <summary>
    ///     Require that the service is registered.
    /// </summary>
    /// <typeparam name="TService">The service that must be registered</typeparam>
    /// <remarks>This version should be used if there is any async or observables registered in the constructor.</remarks>
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

        return service;
    }

    private static bool IsProxyObject(string fullTypeName)
    {
        return StringComparer.Ordinal.Equals(x: fullTypeName, y: "Castle.Proxies.ObjectProxy") ||
               fullTypeName.StartsWith(value: "Castle.Proxies.ObjectProxy_", comparisonType: StringComparison.Ordinal);
    }
}