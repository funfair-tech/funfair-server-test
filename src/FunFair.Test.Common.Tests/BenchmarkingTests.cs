using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using FunFair.Test.Common.Mocks.Benchmarks;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class BenchmarkingTests : LoggingTestBase
{
    public BenchmarkingTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public async Task Run_BenchmarksAsync()
    {
        (Summary summary, AccumulationLogger logger) = await BenchmarkAsync<ExampleBenchmarks>(
            this.CancellationToken()
        );

        this.Output.WriteLine(logger.GetLog());

        summary.AssertAllocationsAtMost(maximumBytes: 1024 * 1024);
    }

    [Fact]
    public async Task Benchmark_WithCancelledToken_ReturnsCancelledTaskAsync()
    {
        using CancellationTokenSource cancellationTokenSource = new();
        await cancellationTokenSource.CancelAsync();

        // Deliberately not awaited here: a synchronous throw must fail the test before the assertion runs.
        ValueTask<(Summary summary, AccumulationLogger logger)> benchmark = BenchmarkAsync<ExampleBenchmarks>(
            cancellationTokenSource.Token
        );

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await benchmark);
    }
}
