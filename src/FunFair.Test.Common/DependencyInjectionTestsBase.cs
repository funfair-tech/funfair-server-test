using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
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
    protected void RequireService<TService>()
        where TService : class
    {
        TService service = this.GetService<TService>();
        Assert.NotNull(service);
    }
}