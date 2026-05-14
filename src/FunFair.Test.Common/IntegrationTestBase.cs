using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FunFair.Test.Common;

public abstract class IntegrationTestBase : LoggingTestBase
{
    protected IntegrationTestBase(ITestOutputHelper output)
        : base(output) { }

    protected IntegrationTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration
    )
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration) { }

    protected IntegrationTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration,
        Action<IServiceProvider> initializeServices
    )
        : base(
            output: output,
            dependencyInjectionRegistration: dependencyInjectionRegistration,
            initializeServices: initializeServices
        ) { }

    protected internal IServiceProvider ServiceProvider => this.RetrieveDependencyInjectionServiceProvider();

    protected T GetService<T>()
        where T : notnull
    {
        return this.GetServiceFromDependencyInjection<T>();
    }
}
