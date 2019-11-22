using System;
using System.Reflection;
using System.Threading.Tasks;
using FunFair.Test.Common.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Simple base class for tests that need logging or output to the test logs.
    /// </summary>
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
            : this(output, NoDependencyInjectionConfiguration, NoDependencyInjectionInitialization)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">XUnit output</param>
        /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
        protected LoggingTestBase(ITestOutputHelper output, Action<IServiceCollection> dependencyInjectionRegistration)
            : this(output, dependencyInjectionRegistration, NoDependencyInjectionInitialization)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">XUnit output</param>
        /// <param name="dependencyInjectionRegistration">Registers services with dependency injection services.</param>
        /// <param name="initializeServices">Initialises services.</param>
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

            LoggingStartup.AddLoggingSupport(serviceCollection, output);
            dependencyInjectionRegistration(serviceCollection);

            this._serviceProvider = serviceCollection.BuildServiceProvider();

            this._loggerFactory = this._serviceProvider.GetService<ILoggerFactory>();
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

        protected virtual void Dispose(bool disposing)
        {
            IDisposable? disposableLogger = this._logger;
            disposableLogger?.Dispose();

            // This forces the constructor to be executed once per test!
            this._loggerFactory?.Dispose();

            TaskScheduler.UnobservedTaskException -= this.ReportUnhandledException;
        }

        /// <summary>
        ///     Gets the service from Dependency injection.
        /// </summary>
        /// <typeparam name="T">The service </typeparam>
        /// <returns></returns>
        protected internal T GetServiceFromDependencyInjection<T>()
        {
            T service = this._serviceProvider.GetService<T>();

            Assert.True(service != null, $"{typeof(T).FullName} could not be loaded from DI container");

            return service;
        }

        /// <summary>
        ///     Gets a typed logger.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <returns>A logger.</returns>
        protected ILogger<T> GetTypedLogger<T>()
        {
            ILogger<T> logger = this._loggerFactory.CreateLogger<T>();

            if (logger == null)
            {
                throw new NullException($"ILogger<{typeof(T).FullName}> could not be loaded from DI container");
            }

            return logger;
        }

        private DisposableLogger BuildLogger()
        {
            MethodInfo method = typeof(LoggingTestBase).GetMethod(nameof(this.GetTypedLogger), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                                throw new MissingMethodException();

            MethodInfo genericMethod = method.MakeGenericMethod(this.GetType());

            ILogger? logger = genericMethod.Invoke(this, Array.Empty<object>()) as ILogger;

            if (logger == null)
            {
                throw new MissingMethodException();
            }

            return new DisposableLogger(logger);
        }

        private void ReportUnhandledException(object? sender, UnobservedTaskExceptionEventArgs args)
        {
            this.Output.WriteLine("Unhandled Exception: " + args.Exception?.Message);
        }

        private sealed class DisposableLogger : ILogger, IDisposable
        {
            private readonly ILogger _logger;
            private readonly IDisposable _scope;

            public DisposableLogger(ILogger logger)
            {
                this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
                this._scope = this._logger.BeginScope(state: "Test");
            }

            public void Dispose()
            {
                this._scope.Dispose();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                this._logger.Log(logLevel, eventId, state, exception, formatter);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return this._logger.IsEnabled(logLevel);
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return this._logger.BeginScope(state);
            }
        }

        private sealed class LogOutput : ITestOutputHelper
        {
            private readonly ILogger _logger;

            public LogOutput(ILogger logger)
            {
                this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public void WriteLine(string message)
            {
                this._logger.LogDebug(message);
            }

            public void WriteLine(string format, params object[] args)
            {
                this._logger.LogDebug(format, args);
            }
        }
    }
}

