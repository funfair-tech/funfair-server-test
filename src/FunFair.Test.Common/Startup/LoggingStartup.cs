using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    public static IServiceCollection AddLoggingSupport(this IServiceCollection services)
    {
        return services.AddLogging(configure: AddFilters);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not easily testable as uses third party services")]
    public static void InitializeLogging(ILoggerFactory loggerFactory, ITestOutputHelper output)
    {
        // set up Serilog logger
        Logger logger = CreateLogger(output);

        // set up the logger factory
        loggerFactory.AddSerilog(logger: logger, dispose: true);
    }

    private static Logger CreateLogger(ITestOutputHelper output)
    {
        return new LoggerConfiguration().Enrich.FromLogContext()
                                        .Enrich.WithSensitiveDataMasking()
                                        .Enrich.WithDemystifiedStackTraces()
                                        .Enrich.WithAssemblyName()
                                        .Enrich.WithAssemblyVersion()
                                        .Enrich.WithMachineName()
                                        .Enrich.WithProcessId()
                                        .Enrich.WithThreadId()
                                        .WriteTo.TestOutput(testOutputHelper: output, restrictedToMinimumLevel: LogEventLevel.Debug)
                                        .WriteTo.Debug(LogEventLevel.Debug)
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