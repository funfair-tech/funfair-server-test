using FunFair.Test.Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    public static IServiceCollection AddLoggingSupport(this IServiceCollection services)
    {
        return services.AddLogging(configure: AddFilters);
    }

    public static void InitializeLogging(ILoggerFactory loggerFactory, ITestOutputHelper output)
    {
        XUnitLoggerProvider? provider = null;

        try
        {
            provider = new XUnitLoggerProvider(output);
            loggerFactory.AddProvider(provider);
            provider = null;
        }
        finally
        {
            provider?.Dispose();
        }
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder
            .ClearProviders()
            .AddFilter(category: "Microsoft", level: LogLevel.Warning)
            .AddFilter(category: "System.Net.Http.HttpClient", level: LogLevel.Warning)
            .SetMinimumLevel(LogLevel.Trace);
    }
}
