#if DEBUG
using System;
using System.Globalization;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace FunFair.Test.Common.Helpers;

// Must stay behaviourally compatible with BenchmarkingHelpers.Release.cs's Benchmark<T>() (same signature, real result).
internal static class BenchmarkingHelpers
{
    public static (Summary summary, AccumulationLogger logger) Benchmark<T>()
    {
        return (
            summary: new Summary(
                title: typeof(T).FullName ?? string.Empty,
                reports: [],
                hostEnvironmentInfo: HostEnvironmentInfo.GetCurrent(),
                resultsDirectoryPath: string.Empty,
                logFilePath: string.Empty,
                totalTime: TimeSpan.Zero,
                cultureInfo: CultureInfo.InvariantCulture,
                validationErrors: [],
                columnHidingRules: [],
                summaryStyle: SummaryStyle.Default
            ),
            logger: new AccumulationLogger()
        );
    }
}
#endif
