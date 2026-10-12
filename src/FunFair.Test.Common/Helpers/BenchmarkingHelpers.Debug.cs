#if DEBUG
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace FunFair.Test.Common.Helpers;

// Must stay behaviourally compatible with BenchmarkingHelpers.Release.cs's BenchmarkAsync<T>() (same signature, real result).
internal static class BenchmarkingHelpers
{
    public static ValueTask<(Summary summary, AccumulationLogger logger)> BenchmarkAsync<T>(
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        (Summary summary, AccumulationLogger logger) result = (
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

        return ValueTask.FromResult(result);
    }
}
#endif
