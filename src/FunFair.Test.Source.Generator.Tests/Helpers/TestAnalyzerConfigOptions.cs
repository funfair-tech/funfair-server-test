using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FunFair.Test.Source.Generator.Tests.Helpers;

internal sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly ImmutableDictionary<string, string> _values;

    public TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string> values)
    {
        this._values = values.ToImmutableDictionary(KeyComparer);
    }

    public override IEnumerable<string> Keys => this._values.Keys;

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return this._values.TryGetValue(key, out value);
    }
}
