using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace FunFair.Test.Common.Tests;

[SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0012:Make Sealed", Justification = "Benchmarks")]
public class ExampleBenchmarks
{
    private readonly IReadOnlyList<int> _items = [.. Enumerable.Range(start: 0, count: 100)];

    [Benchmark]
    public int LinqSum()
    {
        return this._items.Sum();
    }

    [Benchmark]
    public int ForEachSum()
    {
        int number = 0;

        foreach (int item in this._items)
        {
            number += item;
        }

        return number;
    }
}
