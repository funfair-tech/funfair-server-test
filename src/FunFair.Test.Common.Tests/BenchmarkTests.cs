using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class BenchmarkTests : LoggingTestBase
{
    public BenchmarkTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public void DoBenchmark()
    {
        (Summary summary, AccumulationLogger logger) = Benchmark<ExampleBenchmarks>();

        this.Output.WriteLine(summary.Title);

        this.Output.WriteLine(logger.GetLog());
    }
}