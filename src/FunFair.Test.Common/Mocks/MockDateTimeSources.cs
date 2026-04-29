using System;
using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.Time.Testing;

namespace FunFair.Test.Common.Mocks;

public static class MockDateTimeSources
{
    public static FakeTimeProvider AdvancingDateTimeUseWithCaution { get; } = Get(TimeSources.Advanceable);

    public static FakeTimeProvider Past { get; } = Get(TimeSources.Past);

    public static FakeTimeProvider Future { get; } = Get(TimeSources.Future);

    public static FakeTimeProvider Get(in FrozenTimeSource startTime)
    {
        return new(startTime.UtcNowAsOffset);
    }

    public static FakeTimeProvider Get(in AdvanceableTimeSource startTime)
    {
        FakeTimeProvider provider = new(startTime.UtcNowAsOffset);
        provider.AutoAdvanceAmount = TimeSpan.FromDays(1);

        return provider;
    }
}
