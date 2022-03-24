using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    public static IServiceCollection AddLoggingSupport(this IServiceCollection services)
    {
        return services.AddLogging(configure: builder => AddFilters(builder: builder));
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not easily testable as uses third party services")]
    public static void InitializeLogging(ILoggerFactory loggerFactory, ITestOutputHelper output)
    {
        // set up Serilog logger
        Log.Logger = CreateLogger(output);

        // set up the logger factory
        loggerFactory.AddSerilog(dispose: true);
    }

    private static Logger CreateLogger(ITestOutputHelper output)
    {
        return new LoggerConfiguration().Enrich.FromLogContext()
                                        .Enrich.WithDemystifiedStackTraces()
                                        .Enrich.WithMachineName()
                                        .Enrich.WithProcessId()
                                        .Enrich.WithThreadId()
                                        .WriteTo.Xunit(output)
                                        .CreateLogger();
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder.ClearProviders()
               .AddFilter(category: @"Microsoft", level: LogLevel.Warning)
               .AddFilter(category: @"System.Net.Http.HttpClient", level: LogLevel.Warning)
               .SetMinimumLevel(LogLevel.Trace);
    }
}