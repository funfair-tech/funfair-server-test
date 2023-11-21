using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FunFair.Test.Common.Helpers;

[DebuggerDisplay("UtcNow: {UtcNowAsOffset}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct AdvanceableTimeSource
{
    private readonly int _offset;

    private AdvanceableTimeSource(in DateTimeOffset utcNow, int offset)
    {
        this._offset = offset;
        this.BaseOffset = utcNow;
        this.UtcNowAsOffset = utcNow.AddDays(offset);
    }

    public DateTimeOffset BaseOffset { get; }

    public DateTimeOffset UtcNowAsOffset { get; }

    public DateTime UtcNow => this.UtcNowAsOffset.UtcDateTime;

    public AdvanceableTimeSource Next()
    {
        return new(utcNow: this.BaseOffset, this._offset + 1);
    }

    internal static AdvanceableTimeSource Create(in DateTimeOffset utcNow)
    {
        return new(utcNow: utcNow, offset: 0);
    }
}