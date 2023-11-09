using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Common.Helpers;

namespace FunFair.Test.Common;

public static class TimeSources
{
    public static FrozenTimeSource Past { get; } = CreateFrozen(new(year: 1975, month: 03, day: 16, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc));

    public static FrozenTimeSource Future { get; } = CreateFrozen(new(year: 2100, month: 3, day: 16, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc));

    public static AdvanceableTimeSource Advanceable { get; } = CreateAdvanceable(Past);

    public static FrozenTimeSource CreateFrozen(in DateTime startTime)
    {
        return startTime.Kind == DateTimeKind.Utc
            ? CreateFrozen(new DateTimeOffset(dateTime: startTime, offset: TimeSpan.Zero))
            : FrozenMustBeInUtcFormat(startTime);
    }

    public static FrozenTimeSource CreateFrozen(in DateTimeOffset startTime)
    {
        return startTime.Offset == TimeSpan.Zero
            ? FrozenTimeSource.Create(startTime)
            : FrozenMustBeInUtcFormat(startTime);
    }

    public static AdvanceableTimeSource CreateAdvanceable(in DateTime startTime)
    {
        return startTime.Kind == DateTimeKind.Utc
            ? CreateAdvanceable(new DateTimeOffset(dateTime: startTime, offset: TimeSpan.Zero))
            : AdvanceableMustBeInUtcFormat(startTime);
    }

    public static AdvanceableTimeSource CreateAdvanceable(in DateTimeOffset startTime)
    {
        return startTime.Offset == TimeSpan.Zero
            ? AdvanceableTimeSource.Create(startTime)
            : AdvanceableMustBeInUtcFormat(startTime);
    }

    public static AdvanceableTimeSource CreateAdvanceable(in FrozenTimeSource startTime)
    {
        return AdvanceableTimeSource.Create(startTime.UtcNowAsOffset);
    }

    [DoesNotReturn]
    private static FrozenTimeSource FrozenMustBeInUtcFormat(in DateTime startTime)
    {
        throw new ArgumentOutOfRangeException(nameof(startTime), actualValue: startTime, message: "startTime has to be in UTC format");
    }

    [DoesNotReturn]
    private static FrozenTimeSource FrozenMustBeInUtcFormat(in DateTimeOffset startTime)
    {
        throw new ArgumentOutOfRangeException(nameof(startTime), actualValue: startTime, message: "startTime has to be in UTC format with zero offset");
    }

    [DoesNotReturn]
    private static AdvanceableTimeSource AdvanceableMustBeInUtcFormat(in DateTime startTime)
    {
        throw new ArgumentOutOfRangeException(nameof(startTime), actualValue: startTime, message: "startTime has to be in UTC format");
    }

    [DoesNotReturn]
    private static AdvanceableTimeSource AdvanceableMustBeInUtcFormat(in DateTimeOffset startTime)
    {
        throw new ArgumentOutOfRangeException(nameof(startTime), actualValue: startTime, message: "startTime has to be in UTC format with zero offset");
    }
}