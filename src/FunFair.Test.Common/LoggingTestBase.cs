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
    private readonly ILoggerFactory _loggerFactory;

    private readonly IServiceProvider _serviceProvider;

    protected LoggingTestBase(ITestOutputHelper output)
        : this(
            output: output,
            dependencyInjectionRegistration: NoDependencyInjectionConfiguration,
            initializeServices: NoDependencyInjectionInitialization
        ) { }

    protected LoggingTestBase(
        ITestOutputHelper output,
        Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration
    )
        : this(
            output: output,
            dependencyInjectionRegistration: dependencyInjectionRegistration,
            initializeServices: NoDependencyInjectionInitialization
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

        this._serviceProvider = dependencyInjectionRegistration(new ServiceCollection().AddLoggingSupport())
            .BuildServiceProvider();

        this._loggerFactory = this._serviceProvider.GetRequiredService<ILoggerFactory>();
        LoggingStartup.InitializeLogging(loggerFactory: this._loggerFactory, output: output);
        this.Output = output;
        this.Logger = this.GetTypedLogger<LoggingTestBase>();
        initializeServices(this._serviceProvider);
    }

    protected ILogger Logger { get; }

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
            this._loggerFactory.Dispose();

            if (this._serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }

    protected internal T GetServiceFromDependencyInjection<T>()
        where T : notnull
    {
        return this._serviceProvider.GetRequiredService<T>();
    }

    protected internal IServiceProvider RetrieveDependencyInjectionServiceProvider()
    {
        return this._serviceProvider;
    }

    protected sealed override ILogger<T> GetTypedLogger<T>()
    {
        return this._loggerFactory.CreateLogger<T>();
    }

    private void ReportUnhandledException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        this.Output.WriteLine("Unhandled Exception: " + args.Exception.Message);
    }
}
