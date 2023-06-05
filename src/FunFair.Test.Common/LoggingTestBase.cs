using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Test.Common.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

[SuppressMessage(category: "Microsoft.Usage",
                 checkId: "CA2213:DisposableFieldsShouldBeDisposed",
                 MessageId = nameof(_loggerFactory),
                 Justification = "If Disposed then tests can and will report errors")]
public abstract class LoggingTestBase : TestBase
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly IServiceProvider _serviceProvider;

    protected LoggingTestBase(ITestOutputHelper output)
        : this(output: output, dependencyInjectionRegistration: NoDependencyInjectionConfiguration, initializeServices: NoDependencyInjectionInitialization)
    {
    }

    protected LoggingTestBase(ITestOutputHelper output, Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration)
        : this(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration, initializeServices: NoDependencyInjectionInitialization)
    {
    }

    [SuppressMessage(category: "Major Code Smell", checkId: "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "By Design")]
    protected internal LoggingTestBase(ITestOutputHelper output,
                                       Func<IServiceCollection, IServiceCollection> dependencyInjectionRegistration,
                                       Action<IServiceProvider> initializeServices)
    {
        if (output is null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        if (dependencyInjectionRegistration is null)
        {
            throw new ArgumentNullException(nameof(dependencyInjectionRegistration));
        }

        if (initializeServices is null)
        {
            throw new ArgumentNullException(nameof(initializeServices));
        }

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

    protected virtual void Dispose(bool disposing)
    {
        // note do not dispose _loggerFactory in this method
        this._loggerFactory.Dispose();

        TaskScheduler.UnobservedTaskException -= this.ReportUnhandledException;
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