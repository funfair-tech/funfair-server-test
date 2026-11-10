using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Test.Common.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FunFair.Test.Common;

public abstract class LoggingTestBase : TestBase, IDisposable
{
    private static readonly Func<IServiceCollection, IServiceCollection> NoRegistration =
        NoDependencyInjectionConfiguration;

    private static readonly Action<IServiceProvider> NoInitialization = NoDependencyInjectionInitialization;

    private readonly Lazy<ILogger> _logger;

    private readonly Lazy<(IServiceProvider ServiceProvider, ILoggerFactory LoggerFactory)> _services;

    protected LoggingTestBase(ITestOutputHelper output)
        : this(output: output, dependencyInjectionRegistration: NoRegistration, initializeServices: NoInitialization)
    { }

    protected LoggingTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration
    )
        : this(
            output: output,
            dependencyInjectionRegistration: dependencyInjectionRegistration,
            initializeServices: NoInitialization
        ) { }

    [SuppressMessage(
        category: "Major Code Smell",
        checkId: "S3442:\"abstract\" classes should not have \"public\" constructors",
        Justification = "By Design"
    )]
    protected internal LoggingTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration,
        Action<IServiceProvider> initializeServices
    )
    {
        TaskScheduler.UnobservedTaskException += this.ReportUnhandledException;

        this.Output = output;

        this._services = new Lazy<(IServiceProvider ServiceProvider, ILoggerFactory LoggerFactory)>(() =>
        {
            IServiceProvider serviceProvider = dependencyInjectionRegistration(
                    new ServiceCollection().AddLoggingSupport()
                )
                .BuildServiceProvider();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            LoggingStartup.InitializeLogging(loggerFactory: loggerFactory, output: output);

            return (serviceProvider, loggerFactory);
        });

        this._logger = new Lazy<ILogger>(this.GetTypedLogger<LoggingTestBase>);

        if (
            !ReferenceEquals(dependencyInjectionRegistration, NoRegistration)
            || !ReferenceEquals(initializeServices, NoInitialization)
        )
        {
            // Consumer supplied registration/initialisation: preserve eager semantics.
            initializeServices(this._services.Value.ServiceProvider);
        }
    }

    protected ILogger Logger => this._logger.Value;

    protected ITestOutputHelper Output { get; }

    private static void NoDependencyInjectionInitialization(IServiceProvider serviceProvider)
    {
        // Nothing to do
    }

    private static IServiceCollection NoDependencyInjectionConfiguration(IServiceCollection serviceCollection)
    {
        // Nothing to do
        return serviceCollection;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        TaskScheduler.UnobservedTaskException -= this.ReportUnhandledException;

        if (disposing)
        {
            if (this._services.IsValueCreated)
            {
                this._services.Value.LoggerFactory.Dispose();

                if (this._services.Value.ServiceProvider is IDisposable disposableServiceProvider)
                {
                    disposableServiceProvider.Dispose();
                }
            }
        }
    }

    protected internal T GetServiceFromDependencyInjection<T>()
        where T : notnull
    {
        return this._services.Value.ServiceProvider.GetRequiredService<T>();
    }

    protected internal IServiceProvider RetrieveDependencyInjectionServiceProvider()
    {
        return this._services.Value.ServiceProvider;
    }

    protected sealed override ILogger<T> GetTypedLogger<T>()
    {
        return this._services.Value.LoggerFactory.CreateLogger<T>();
    }

    private void ReportUnhandledException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        this.Output.WriteLine("Unhandled Exception: " + args.Exception.Message);
    }
}
