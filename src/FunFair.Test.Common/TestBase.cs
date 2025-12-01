using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Bogus;
using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common;

public abstract class TestBase
{
    protected TestBase()
    {
        // Nothing to do here!
        Assert.False(condition: false, userMessage: "Because");
    }

    [SuppressMessage("codecracker.CSharp", "CC0091: Make static", Justification = "Simplifies API")]
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected CancellationToken CancellationToken()
    {
        return GetTestCancellationToken();
    }

    protected CancellationTokenSource CreateCancellationTokenSource(in CancellationToken cancellationToken)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(this.CancellationToken(), cancellationToken);
    }

    private static CancellationToken GetTestCancellationToken()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();

        return cancellationToken;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Task<T?> FromOptionalResultAsync<T>(T? value)
        where T : class
    {
        TestContext.Current.CancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(value);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Task<T?> NullResultAsync<T>()
        where T : class
    {
        return FromOptionalResultAsync((T?)null);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual ILogger<T> GetTypedLogger<T>()
    {
        return GetSubstitute<ILogger<T>>();
    }

    protected static IReadOnlyList<T> MakeFake<T>(Func<Faker<T>, Faker<T>> rules, int itemCount)
        where T : class
    {
        Assert.True(itemCount > 0, userMessage: "Must generate at least one ");

        const bool enable = true;

        return rules(new Faker<T>().StrictMode(enable))
            .Generate(itemCount);
    }

    protected static T GetSubstitute<T>(params object[] constructorArguments)
        where T : class
    {
        return Substitute.For<T>(constructorArguments);
    }

    protected static T1 GetSubstitute<T1, T2>(params object[] constructorArguments)
        where T1 : class where T2 : class
    {
        return Substitute.For<T1, T2>(constructorArguments);
    }

    protected static T1 GetSubstitute<T1, T2, T3>(params object[] constructorArguments)
        where T1 : class where T2 : class where T3 : class
    {
        return Substitute.For<T1, T2, T3>(constructorArguments);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static T AssertReallyNotNull<T>([NotNull] T? value)
        where T : class
    {
        Assert.NotNull(value);

        return value;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static T AssertReallyNotNull<T>([NotNull] T? value)
        where T : struct
    {
        return Assert.NotNull(value);
    }

    [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Needed for Unit Test")]
    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0057:ReviewUnusedParameters", Justification = "Needed for Unit Test")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void UnusedVariable<T>(T value)
    {
        // Marking that the variable is unused.
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static string FormatValue<T>(T value)
        where T : notnull
    {
        return value.FormatValue();
    }

    protected static (Summary summary, AccumulationLogger logger) Benchmark<T>()
    {
#if DEBUG
        return (summary: new Summary(), logger: new AccumulationLogger());
#else
        AccumulationLogger logger = new();

        ManualConfig config = ManualConfig.Create(DefaultConfig.Instance)
                                          .AddLogger(logger)
                                          .AddDiagnoser(new MemoryDiagnoser(new(false)))
                                          .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        Summary summary = BenchmarkRunner.Run<T>(config);

        return (summary, logger);
#endif
    }
}