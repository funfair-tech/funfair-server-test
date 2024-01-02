using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using Serilog.Exceptions;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup;

internal static class LoggingStartup
{
    private static readonly List<IMaskingOperator> MaskingOperators =
    [
        new EmailAddressMaskingOperator(), new CreditCardMaskingOperator(), new IbanMaskingOperator()

        // need to find a sane way of adding these
    ];

    public static IServiceCollection AddLoggingSupport(this IServiceCollection services)
    {
        return services.AddLogging(configure: AddFilters);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "SmartAnalyzers.CSharpExtensions.Annotations", checkId: "CSE007:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    public static void InitializeLogging(ILoggerFactory loggerFactory, ITestOutputHelper output)
    {
        // set up Serilog logger
        Logger logger = CreateLogger(output);

        // set up the logger factory
        loggerFactory.AddSerilog(logger: logger, dispose: true);
    }

    private static Logger CreateLogger(ITestOutputHelper output)
    {
        return new LoggerConfiguration().Enrich()
                                        .WithOutput(output)
                                        .CreateLogger();
    }

    private static LoggerConfiguration Enrich(this LoggerConfiguration configuration)
    {
        return configuration.Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails()
                            .Enrich.WithSensitiveDataMasking(MaskingOptions)
                            .Enrich.WithDemystifiedStackTraces()
                            .Enrich.WithAssemblyName()
                            .Enrich.WithAssemblyVersion()
                            .Enrich.WithEnvironmentName()
                            .Enrich.WithEnvironmentUserName()
                            .Enrich.WithMachineName()
                            .Enrich.WithProcessId()
                            .Enrich.WithThreadId();
    }

    private static void MaskingOptions(SensitiveDataEnricherOptions options)
    {
        options.Mode = MaskingMode.Globally;
        options.MaskingOperators = MaskingOperators;
        options.MaskValue = "**MASKED*";
    }

    private static LoggerConfiguration WithOutput(this LoggerConfiguration configuration, ITestOutputHelper output)
    {
        return configuration.WriteTo.TestOutput(testOutputHelper: output, restrictedToMinimumLevel: LogEventLevel.Debug)
                            .WriteTo.Debug(LogEventLevel.Debug);
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder.ClearProviders()
               .AddFilter(category: "Microsoft", level: LogLevel.Warning)
               .AddFilter(category: "System.Net.Http.HttpClient", level: LogLevel.Warning)
               .SetMinimumLevel(LogLevel.Trace);
    }
}