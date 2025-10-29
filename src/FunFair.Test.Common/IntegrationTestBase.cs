using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FunFair.Test.Common;


public abstract class IntegrationTestBase : LoggingTestBase
{
    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedMember.Global",
        Justification = "May be used in derived test classes"
    )]
    protected IntegrationTestBase(ITestOutputHelper output)
        : base(output) { }

    protected IntegrationTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration
    )
        : base(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration) { }

    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedMember.Global",
        Justification = "May be used in derived test classes"
    )]
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

    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedMember.Global",
        Justification = "May be used in derived test classes"
    )]
    protected internal IServiceProvider ServiceProvider => this.RetrieveDependencyInjectionServiceProvider();

    [SuppressMessage(
        category: "ReSharper",
        checkId: "UnusedMember.Global",
        Justification = "May be used in derived test classes"
    )]
    protected T GetService<T>()
        where T : notnull
    {
        return this.GetServiceFromDependencyInjection<T>();
    }
}
