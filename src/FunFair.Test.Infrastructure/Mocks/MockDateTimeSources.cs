using System;
using Microsoft.Extensions.Time.Testing;

namespace FunFair.Test.Infrastructure.Mocks;

public static class MockDateTimeSources
{
    private static readonly DateTimeOffset PastDate = new(
        year: 1975,
        month: 3,
        day: 16,
        hour: 0,
        minute: 0,
        second: 0,
        offset: TimeSpan.Zero
    );
    private static readonly DateTimeOffset FutureDate = new(
        year: 2100,
        month: 3,
        day: 16,
        hour: 0,
        minute: 0,
        second: 0,
        offset: TimeSpan.Zero
    );

    public static FakeTimeProvider Past => new(PastDate);

    public static FakeTimeProvider Future => new(FutureDate);

    public static FakeTimeProvider AdvancingDateTimeUseWithCaution =>
        new(PastDate) { AutoAdvanceAmount = TimeSpan.FromDays(1) };
}
