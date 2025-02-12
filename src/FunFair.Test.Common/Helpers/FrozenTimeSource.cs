using System;
using System.Diagnostics;

namespace FunFair.Test.Common.Helpers;

[DebuggerDisplay("UtcNow: {UtcNowAsOffset}")]
public readonly record struct FrozenTimeSource
{
    private FrozenTimeSource(in DateTimeOffset utcNow)
    {
        this.UtcNowAsOffset = utcNow;
    }

    public DateTimeOffset UtcNowAsOffset { get; }

    public DateTime UtcNow => this.UtcNowAsOffset.UtcDateTime;

    internal static FrozenTimeSource Create(in DateTimeOffset utcNow)
    {
        return new(utcNow);
    }
}
