using System.Diagnostics.CodeAnalysis;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    public static IServiceCollection AddLoggingSupport(this IServiceCollection services)
    {
        return services.AddLogging(configure: AddFilters);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "SmartAnalyzers.CSharpExtensions.Annotations", checkId: "CSE007:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    public static void InitializeLogging(ILoggerFactory loggerFactory, ITestOutputHelper output)
    {
        loggerFactory.AddProvider(new XUnitLoggerProvider(output));
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder.ClearProviders()
               .AddFilter(category: "Microsoft", level: LogLevel.Warning)
               .AddFilter(category: "System.Net.Http.HttpClient", level: LogLevel.Warning)
               .SetMinimumLevel(LogLevel.Trace);
    }
}