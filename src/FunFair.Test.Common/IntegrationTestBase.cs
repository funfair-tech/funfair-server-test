using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

/// <summary>
///     Simple base class for integration tests..
/// </summary>
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
public abstract class IntegrationTestBase : LoggingTestBase
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">XUnit output</param>
    protected IntegrationTestBase(ITestOutputHelper output)
        : base(output)
    {
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">XUnit output</param>
    /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
    protected IntegrationTestBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration)
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration)
    {
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">XUnit output</param>
    /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
    /// <param name="initializeServices">Initialises services.</param>
    protected IntegrationTestBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration, Action<IServiceProvider> initializeServices)
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration, initializeServices: initializeServices)
    {
    }

    /// <summary>
    ///     Gets the Dependency Injection Service Provider.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    protected internal IServiceProvider ServiceProvider => this.RetrieveDependencyInjectionServiceProvider();

    /// <summary>
    ///     Gets the service from Dependency injection.
    /// </summary>
    /// <typeparam name="T">The service </typeparam>
    /// <returns>The service that was registered with dependency injection.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    protected T GetService<T>()
        where T : notnull
    {
        return this.GetServiceFromDependencyInjection<T>();
    }
}