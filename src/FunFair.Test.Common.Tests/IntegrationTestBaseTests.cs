using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class IntegrationTestBaseTests : IntegrationTestBase
{
    public IntegrationTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void ServiceProviderIsAccessible()
    {
        IServiceProvider serviceProvider = this.ServiceProvider;
        this.Output.WriteLine(serviceProvider.GetType().FullName ?? string.Empty);
    }

    [Fact]
    public void CanGetService()
    {
        ILoggerFactory loggerFactory = this.GetService<ILoggerFactory>();
        this.Output.WriteLine(loggerFactory.GetType().FullName ?? string.Empty);
    }
}
