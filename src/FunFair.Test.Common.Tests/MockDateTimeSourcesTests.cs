using System;
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
        DateTimeOffset expected = new(
            year: 1975,
            month: 3,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );

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
    public void PastTimeProviderCreatesNewInstanceEachCall()
    {
        FakeTimeProvider first = MockDateTimeSources.Past;
        FakeTimeProvider second = MockDateTimeSources.Past;

        Assert.NotSame(expected: first, actual: second);
    }

    [Fact]
    public void FutureTimeProviderIsFrozenAtExpectedTime()
    {
        DateTimeOffset expected = new(
            year: 2100,
            month: 3,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );

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
    public void FutureTimeProviderCreatesNewInstanceEachCall()
    {
        FakeTimeProvider first = MockDateTimeSources.Future;
        FakeTimeProvider second = MockDateTimeSources.Future;

        Assert.NotSame(expected: first, actual: second);
    }

    [Fact]
    public void AdvancingTimeProviderStartsAtExpectedTime()
    {
        DateTimeOffset expected = new(
            year: 1975,
            month: 3,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );

        Assert.Equal(
            expected: expected,
            actual: MockDateTimeSources.AdvancingDateTimeUseWithCaution.GetUtcNow()
        );
    }

    [Fact]
    public void AdvancingTimeProviderAdvancesByOneDayPerCall()
    {
        FakeTimeProvider provider = MockDateTimeSources.AdvancingDateTimeUseWithCaution;

        DateTimeOffset first = provider.GetUtcNow();
        DateTimeOffset second = provider.GetUtcNow();

        Assert.Equal(expected: TimeSpan.FromDays(1), actual: second - first);
    }

    [Fact]
    public void AdvancingTimeProviderCreatesNewInstanceEachCall()
    {
        FakeTimeProvider first = MockDateTimeSources.AdvancingDateTimeUseWithCaution;
        FakeTimeProvider second = MockDateTimeSources.AdvancingDateTimeUseWithCaution;

        Assert.NotSame(expected: first, actual: second);
    }

    [Fact]
    public void AdvancingTimeProviderInstancesStartAtSameTime()
    {
        FakeTimeProvider first = MockDateTimeSources.AdvancingDateTimeUseWithCaution;
        FakeTimeProvider second = MockDateTimeSources.AdvancingDateTimeUseWithCaution;

        Assert.Equal(expected: first.GetUtcNow(), actual: second.GetUtcNow());
    }
}
