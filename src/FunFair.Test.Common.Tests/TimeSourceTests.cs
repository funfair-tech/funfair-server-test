using System;
using FunFair.Test.Common.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

public sealed class TimeSourceTests : LoggingTestBase
{
    public TimeSourceTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void PastDateTimeIsSane()
    {
        DateTime expected = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc
        );
        Assert.Equal(expected: expected, actual: TimeSources.Past.UtcNow);
    }

    [Fact]
    public void PastDateTimeOffsetIsSane()
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
        Assert.Equal(expected: expected, actual: TimeSources.Past.UtcNowAsOffset);
    }

    [Fact]
    public void FutureDateTimeIsSane()
    {
        DateTime expected = new(
            year: 2100,
            month: 3,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc
        );
        Assert.Equal(expected: expected, actual: TimeSources.Future.UtcNow);
    }

    [Fact]
    public void FutureDateTimeOffsetIsSane()
    {
        DateTimeOffset expected = new(
            year: 2100,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );
        Assert.Equal(expected: expected, actual: TimeSources.Future.UtcNowAsOffset);
    }

    [Fact]
    public void AdvanceableInitialDateTimeIsSane()
    {
        DateTime expected = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc
        );
        Assert.Equal(expected: expected, actual: TimeSources.Advanceable.UtcNow);
    }

    [Fact]
    public void AdvanceableInitialDateTimeOffsetIsSane()
    {
        DateTimeOffset expected = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );
        Assert.Equal(expected: expected, actual: TimeSources.Advanceable.UtcNowAsOffset);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    public void AdvanceableDateTimeIsSaneAtOffset(int offset)
    {
        DateTime baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Utc
        );
        DateTime expected = baseTime.AddDays(offset);

        AdvanceableTimeSource current = TimeSources.Advanceable;

        while (offset > 0)
        {
            AdvanceableTimeSource last = current;

            current = current.Next();
            this.Output.WriteLine($"Advancing from {last.UtcNow} to {current.UtcNow}");
            --offset;
        }

        this.Output.WriteLine($"Final time: {current.UtcNow}");

        Assert.Equal(expected: expected, actual: current.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    public void AdvanceableDateTimeOffsetIsSaneAtOffset(int offset)
    {
        DateTimeOffset baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero
        );
        DateTimeOffset expected = baseTime.AddDays(offset);

        AdvanceableTimeSource current = TimeSources.Advanceable;

        while (offset > 0)
        {
            AdvanceableTimeSource last = current;

            current = current.Next();
            this.Output.WriteLine(
                $"Advancing from {last.UtcNowAsOffset} to {current.UtcNowAsOffset}"
            );
            --offset;
        }

        this.Output.WriteLine($"Final time: {current.UtcNowAsOffset}");

        Assert.Equal(expected: expected, actual: current.UtcNowAsOffset);
    }

    [Fact]
    public void CannotCreateFrozenTimeSourceInLocaltime()
    {
        DateTime baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Local
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeSources.CreateFrozen(baseTime));
    }

    [Fact]
    public void CannotCreateFrozenTimeSourceInUnspecified()
    {
        DateTime baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Unspecified
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeSources.CreateFrozen(baseTime));
    }

    [Fact]
    public void CannotCreateAdvanceableTimeSourceInLocaltime()
    {
        DateTime baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Local
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeSources.CreateAdvanceable(baseTime));
    }

    [Fact]
    public void CannotCreateAdvanceableTimeSourceInUnspecified()
    {
        DateTime baseTime = new(
            year: 1975,
            month: 03,
            day: 16,
            hour: 0,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Unspecified
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeSources.CreateAdvanceable(baseTime));
    }
}
