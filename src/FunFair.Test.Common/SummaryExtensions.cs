using System;
using BenchmarkDotNet.Reports;
using Xunit;

namespace FunFair.Test.Common;

public static class SummaryExtensions
{
    private const string AllocatedMemoryMetricId = "Allocated Memory";

    public static void AssertNoAllocations(this Summary summary)
    {
        AssertMaxAllocations(summary: summary, maximumBytes: 0, benchmarkName: null);
    }

    public static void AssertNoAllocations(this Summary summary, string benchmarkName)
    {
        AssertMaxAllocations(summary: summary, maximumBytes: 0, benchmarkName: benchmarkName);
    }

    public static void AssertAllocationsAtMost(this Summary summary, long maximumBytes)
    {
        AssertMaxAllocations(summary: summary, maximumBytes: maximumBytes, benchmarkName: null);
    }

    public static void AssertAllocationsAtMost(
        this Summary summary,
        string benchmarkName,
        long maximumBytes
    )
    {
        AssertMaxAllocations(
            summary: summary,
            maximumBytes: maximumBytes,
            benchmarkName: benchmarkName
        );
    }

    private static void AssertMaxAllocations(
        Summary summary,
        long maximumBytes,
        string? benchmarkName
    )
    {
        bool foundNamed = false;

        foreach (BenchmarkReport report in summary.Reports)
        {
            string name = report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo;

            if (
                benchmarkName is not null
                && !string.Equals(
                    a: name,
                    b: benchmarkName,
                    comparisonType: StringComparison.Ordinal
                )
            )
            {
                continue;
            }

            foundNamed = true;

            if (
                !report.Metrics.TryGetValue(key: AllocatedMemoryMetricId, value: out Metric? metric)
                || metric is null
            )
            {
                continue;
            }

            long allocated = (long)metric.Value;

            Assert.True(
                condition: allocated <= maximumBytes,
                userMessage: $"Benchmark '{name}' allocated {allocated} bytes but the maximum allowed is {maximumBytes} bytes"
            );
        }

        if (benchmarkName is not null && !summary.Reports.IsEmpty)
        {
            Assert.True(
                condition: foundNamed,
                userMessage: $"No benchmark named '{benchmarkName}' was found in the summary"
            );
        }
    }
}
