using System;
using FunFair.Test.Common.Helpers;
using FunFair.Test.Common.Mocks;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class MockDateTimeSourcesTests : LoggingTestBase
{
    public MockDateTimeSourcesTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void PastTimeProviderIsFrozenAtExpectedTime()
    {
        DateTimeOffset expected = new(year: 1975, month: 3, day: 16, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero);

        Assert.Equal(expected: expected, actual: MockDateTimeSources.Past.GetUtcNow());
    }

    [Fact]
    public void PastTimeProviderReturnsSameTimeOnRepeatedCalls()
    {
        FakeTimeProvider provider = MockDateTimeSources.Past;

        DateTimeOffset first = provider.GetUtcNow();
        DateTimeOffset second = provider.GetUtcNow();

        Assert.Equal(expected: first, actual: second);
    }

    [Fact]
    public void FutureTimeProviderIsFrozenAtExpectedTime()
    {
        DateTimeOffset expected = new(year: 2100, month: 3, day: 16, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero);

        Assert.Equal(expected: expected, actual: MockDateTimeSources.Future.GetUtcNow());
    }

    [Fact]
    public void FutureTimeProviderReturnsSameTimeOnRepeatedCalls()
    {
        FakeTimeProvider provider = MockDateTimeSources.Future;

        DateTimeOffset first = provider.GetUtcNow();
        DateTimeOffset second = provider.GetUtcNow();

        Assert.Equal(expected: first, actual: second);
    }

    [Fact]
    public void GetWithFrozenTimeSourceReturnsCorrectTime()
    {
        FrozenTimeSource frozenTime = TimeSources.Past;
        FakeTimeProvider provider = MockDateTimeSources.Get(frozenTime);

        Assert.Equal(expected: frozenTime.UtcNowAsOffset, actual: provider.GetUtcNow());
    }

    [Fact]
    public void GetWithFrozenTimeSourceReturnsSameTimeOnRepeatedCalls()
    {
        FakeTimeProvider provider = MockDateTimeSources.Get(TimeSources.Past);

        DateTimeOffset first = provider.GetUtcNow();
        DateTimeOffset second = provider.GetUtcNow();

        Assert.Equal(expected: first, actual: second);
    }

    [Fact]
    public void GetWithFrozenTimeSourceCreatesNewInstanceEachCall()
    {
        FakeTimeProvider first = MockDateTimeSources.Get(TimeSources.Past);
        FakeTimeProvider second = MockDateTimeSources.Get(TimeSources.Past);

        Assert.NotSame(expected: first, actual: second);
    }

    [Fact]
    public void GetWithAdvanceableTimeSourceStartsAtCorrectTime()
    {
        AdvanceableTimeSource advanceable = TimeSources.Advanceable;
        FakeTimeProvider provider = MockDateTimeSources.Get(advanceable);

        Assert.Equal(expected: advanceable.UtcNowAsOffset, actual: provider.GetUtcNow());
    }

    [Fact]
    public void GetWithAdvanceableTimeSourceAdvancesByOneDayPerCall()
    {
        FakeTimeProvider provider = MockDateTimeSources.Get(TimeSources.Advanceable);

        DateTimeOffset first = provider.GetUtcNow();
        DateTimeOffset second = provider.GetUtcNow();

        Assert.Equal(expected: TimeSpan.FromDays(1), actual: second - first);
    }

    [Fact]
    public void GetWithAdvanceableTimeSourceCreatesNewInstanceEachCall()
    {
        FakeTimeProvider first = MockDateTimeSources.Get(TimeSources.Advanceable);
        FakeTimeProvider second = MockDateTimeSources.Get(TimeSources.Advanceable);

        Assert.NotSame(expected: first, actual: second);
    }

    [Fact]
    public void GetWithAdvanceableTimeSourceEachInstanceStartsAtSameTime()
    {
        FakeTimeProvider first = MockDateTimeSources.Get(TimeSources.Advanceable);
        FakeTimeProvider second = MockDateTimeSources.Get(TimeSources.Advanceable);

        Assert.Equal(expected: first.GetUtcNow(), actual: second.GetUtcNow());
    }
}
