using FunFair.Test.Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    public static IServiceCollection AddLoggingSupport(this IServiceCollection services, ITestOutputHelper output)
    {
        return services.AddLogging(configure: builder => AddFilters(builder: builder, output: output));
    }

    private static void AddFilters(ILoggingBuilder builder, ITestOutputHelper output)
    {
        builder.ClearProviders()
               .AddXUnit(output)
               .AddFilter(category: @"Microsoft", level: LogLevel.Warning)
               .AddFilter(category: @"System.Net.Http.HttpClient", level: LogLevel.Warning)
               .SetMinimumLevel(LogLevel.Trace);
    }
}