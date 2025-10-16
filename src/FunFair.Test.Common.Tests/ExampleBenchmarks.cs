using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace FunFair.Test.Common.Tests;

[SuppressMessage(
    category: "Microsoft.Performance",
    checkId: "CA1812:AvoidUninstantiatedInternalClasses",
    Justification = "Benchmarks"
)]
public abstract class ExampleBenchmarks
{
    private readonly IReadOnlyList<int> _items = [.. Enumerable.Range(start: 0, count: 100)];

    [Benchmark]
    public int LinqSum()
    {
        return this._items.Sum();
    }
}
