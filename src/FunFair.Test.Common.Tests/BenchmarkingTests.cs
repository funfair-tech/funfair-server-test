using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class BenchmarkingTests : LoggingTestBase
{
    public BenchmarkingTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void Run_Benchmarks()
    {
        (Summary _, AccumulationLogger logger) = Benchmark<ExampleBenchmarks>();

        this.Output.WriteLine(logger.GetLog());
    }
}
