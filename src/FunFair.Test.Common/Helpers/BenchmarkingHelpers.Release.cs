#if !DEBUG
using System;
using System.Globalization;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Xunit;

namespace FunFair.Test.Common.Helpers;

internal static partial class BenchmarkingHelpers
{
    private const string BENCHMARK_BUILD_TIMEOUT_SECONDS_ENVIRONMENT_VARIABLE =
        "FUNFAIR_TEST_BENCHMARK_BUILD_TIMEOUT_SECONDS";

    private static readonly TimeSpan DefaultBenchmarkBuildTimeout = TimeSpan.FromMinutes(10);

    public static (Summary summary, AccumulationLogger logger) Benchmark<T>()
    {
        AccumulationLogger logger = new();

        ManualConfig config = ManualConfig
            .Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .AddDiagnoser(new MemoryDiagnoser(new(displayGenColumns: false)))
            .WithOptions(ConfigOptions.StopOnFirstError)
            .WithBuildTimeout(GetBenchmarkBuildTimeout());

        Summary summary = BenchmarkRunner.Run<T>(config);

        Assert.False(condition: summary.HasCriticalValidationErrors, logger.GetLog());

        return (summary, logger);
    }

    private static TimeSpan GetBenchmarkBuildTimeout()
    {
        string? value = Environment.GetEnvironmentVariable(BENCHMARK_BUILD_TIMEOUT_SECONDS_ENVIRONMENT_VARIABLE);

        if (
            !string.IsNullOrEmpty(value)
            && int.TryParse(value, style: NumberStyles.Integer, provider: CultureInfo.InvariantCulture, out int seconds)
            && seconds > 0
        )
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return DefaultBenchmarkBuildTimeout;
    }
}
#endif
