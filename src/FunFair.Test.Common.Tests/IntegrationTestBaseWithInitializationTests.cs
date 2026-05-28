using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class IntegrationTestBaseWithInitializationTests : IntegrationTestBase
{
    public IntegrationTestBaseWithInitializationTests(ITestOutputHelper output)
        : base(
            output: output,
            dependencyInjectionRegistration: static serviceCollection => serviceCollection,
            initializeServices: static _ => { }
        ) { }

    [Fact]
    public void ServiceProviderIsAccessibleAfterInitialization()
    {
        IServiceProvider serviceProvider = this.ServiceProvider;
        this.Output.WriteLine(serviceProvider.GetType().FullName ?? string.Empty);
    }

    [Fact]
    public void CanGetServiceAfterInitialization()
    {
        ILoggerFactory loggerFactory = this.GetService<ILoggerFactory>();
        this.Output.WriteLine(loggerFactory.GetType().FullName ?? string.Empty);
    }
}
