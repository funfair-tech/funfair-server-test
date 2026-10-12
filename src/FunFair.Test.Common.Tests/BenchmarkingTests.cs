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
            TestContext.Current.CancellationToken
        );

        this.Output.WriteLine(logger.GetLog());

        summary.AssertAllocationsAtMost(maximumBytes: 1024 * 1024);
    }
}
