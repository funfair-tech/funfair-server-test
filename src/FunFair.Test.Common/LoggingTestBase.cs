using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Test.Common.Logging;
using FunFair.Test.Common.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Simple base class for tests that need logging or output to the test logs.
    /// </summary>

    [SuppressMessage(category: "Microsoft.Usage",
                     checkId: "CA2213:DisposableFieldsShouldBeDisposed",
                     MessageId = "_loggerFactory",
                     Justification = "If Disposed then tests can and will report errors")]
    public abstract class LoggingTestBase : TestBase, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;

        private readonly IServiceProvider _serviceProvider;

        private DisposableLogger? _logger;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">XUnit output</param>
        protected LoggingTestBase(ITestOutputHelper output)
            : this(output: output, dependencyInjectionRegistration: NoDependencyInjectionConfiguration, initializeServices: NoDependencyInjectionInitialization)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">XUnit output</param>
        /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
        protected LoggingTestBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration)
            : this(output: output, dependencyInjectionRegistration: dependencyInjectionRegistration, initializeServices: NoDependencyInjectionInitialization)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">XUnit output</param>
        /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
        /// <param name="initializeServices">Initialises services.</param>

        [SuppressMessage(category: "Major Code Smell", checkId: "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "By Design")]
        protected internal LoggingTestBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration, Action<IServiceProvider> initializeServices)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (dependencyInjectionRegistration == null)
            {
                throw new ArgumentNullException(nameof(dependencyInjectionRegistration));
            }

            if (initializeServices == null)
            {
                throw new ArgumentNullException(nameof(initializeServices));
            }

            TaskScheduler.UnobservedTaskException += this.ReportUnhandledException;

            IServiceCollection serviceCollection = new ServiceCollection();

            LoggingStartup.AddLoggingSupport(services: serviceCollection, output: output);
            dependencyInjectionRegistration(serviceCollection);

            this._serviceProvider = serviceCollection.BuildServiceProvider();

            this._loggerFactory = this._serviceProvider.GetRequiredService<ILoggerFactory>();
            initializeServices(this._serviceProvider);
        }

        /// <summary>
        ///     Gets a logger
        /// </summary>
        protected ILogger Logger => this._logger ??= this.BuildLogger();

        /// <summary>
        ///     Test Log output.
        /// </summary>
        protected ITestOutputHelper Output => new LogOutput(this.Logger);

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static void NoDependencyInjectionInitialization(IServiceProvider serviceProvider)
        {
            // Nothing to do
        }

        private static void NoDependencyInjectionConfiguration(IServiceCollection serviceCollection)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Disposes of any managed resources
        /// </summary>
        /// <param name="disposing">true, when the object is being disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            IDisposable? disposableLogger = this._logger;
            disposableLogger?.Dispose();

            // note do not dispose _loggerFactory in this method

            TaskScheduler.UnobservedTaskException -= this.ReportUnhandledException;
        }

        /// <summary>
        ///     Gets the service from Dependency injection.
        /// </summary>
        /// <typeparam name="T">The service </typeparam>
        /// <returns></returns>
        protected internal T GetServiceFromDependencyInjection<T>()
            where T : notnull
        {
            T service = this._serviceProvider.GetRequiredService<T>();

            Assert.True(service != null, $"{typeof(T).FullName} could not be loaded from DI container");

            return service!;
        }

        /// <summary>
        ///     Gets the Service provider that's registered.
        /// </summary>
        /// <returns></returns>
        protected internal IServiceProvider RetrieveDependencyInjectionServiceProvider()
        {
            return this._serviceProvider;
        }

        /// <summary>
        ///     Gets a typed logger.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <returns>A logger.</returns>
        protected ILogger<T> GetTypedLogger<T>()
        {
            return this._loggerFactory.CreateLogger<T>();
        }

        private DisposableLogger BuildLogger()
        {
            return new(this.GetTypedLogger<LoggingTestBase>());
        }

        private void ReportUnhandledException(object? sender, UnobservedTaskExceptionEventArgs args)
        {
            this.Output.WriteLine("Unhandled Exception: " + args.Exception.Message);
        }
    }
}


