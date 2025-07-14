namespace Quantum.Tempo.Tests;

using System;
using Xunit;
using TimeSequencer.Times;

public class TimeTests
{
    [Theory]
    [InlineData("2017-04-09", 1, "2017-04-10")]
    [InlineData("2017-04-09", 2, "2017-04-11")]
    [InlineData("2017-04-09", 22, "2017-05-01")]
    [InlineData("2017-04", 1, "2017-05")]
    [InlineData("2017-12", 1, "2018-01")]
    [InlineData("2017-12", 2, "2018-02")]
    [InlineData("2017", 1, "2018")]
    [InlineData("2017", 2, "2019")]
    [InlineData("2017-04-09T11:17", 1, "2017-04-09T11:18")]
    [InlineData("2017-04-09T11:17", 3, "2017-04-09T11:20")]
    [InlineData("2017-04-09T11:17", 53, "2017-04-09T12:10")]
    [InlineData("2017-04-09T11:17", 43, "2017-04-09T12:00")]
    [InlineData("2017-02-28", 1, "2017-03-01")]
    [InlineData("2017-02-28", 2, "2017-03-02")]
    [InlineData("2016-02-28", 1, "2016-02-29")]
    [InlineData("2016-02-28", 2, "2016-03-01")]

    [InlineData("2017-070", 1, "2017-071")]
    [InlineData("2017-070", 10, "2017-080")]
    [InlineData("2017-365", 1, "2018-001")]

    [InlineData("2017-W52", 1, "2018-W01")]
    [InlineData("2017-W50", 1, "2017-W51")]
    [InlineData("2017-W50", 2, "2017-W52")]
    [InlineData("2017-W50", 3, "2018-W01")]
    public void next_time(string actual, int nextTimes, string expected)
        => Assert.Equal(expected, actual.FromIso().Next(nextTimes));

    [Theory]

    [InlineData("2017", 3, "2014")]

    [InlineData("2017-W51", 1, "2017-W50")]
    [InlineData("2017-W52", 52, "2016-W52")]
    [InlineData("2017-W10", 8, "2017-W02")]

    [InlineData("2017-02-23", 5, "2017-02-18")]
    [InlineData("2017-02-23", 24, "2017-01-31")]
    [InlineData("2017-02-01", 32, "2016-12-31")]

    [InlineData("2017-02-23T20:30", 6, "2017-02-23T20:24")]
    [InlineData("2017-02-23T20:10", 11, "2017-02-23T19:59")]
    public void test(string expected, int nextTimes, string actual)
    {
        Assert.Equal(expected, actual.FromIso().Next(nextTimes));
    }

    [Theory]
    [InlineData("2017", 3, "2014")]
    [InlineData("2017-W51", 1, "2017-W50")]
    [InlineData("2017-W52", 52, "2016-W52")]
    [InlineData("2017-W10", 8, "2017-W02")]
    [InlineData("2017-02-23", 5, "2017-02-18")]
    [InlineData("2017-02-23", 24, "2017-01-31")]
    [InlineData("2017-02-01", 32, "2016-12-31")]
    [InlineData("2017-02-23T20:30", 6, "2017-02-23T20:24")]
    [InlineData("2017-02-23T20:10", 11, "2017-02-23T19:59")]
    [InlineData("2017-360", 5, "2017-355")]
    [InlineData("2017-005", 4, "2017-001")]
    [InlineData("2017-005", 5, "2016-365")]
    [InlineData("2017-005", 6, "2016-364")]

    [InlineData("2017-W52-1", 2, "2017-W51-6")]
    [InlineData("2017-W52-7", 7, "2017-W51-7")]
    [InlineData("2017-W52-7", 14, "2017-W50-7")]
    public void prev_time(string actual, int prevTimes, string expected)
        => Assert.Equal(expected, actual.FromIso().Prev(prevTimes));

    [Theory]
    [InlineData("2017-W52-1", 2, "2017-W51-6")]
    [InlineData("2017-W52-7", 7, "2017-W51-7")]
    [InlineData("2017-W52-7", 14, "2017-W50-7")]
    public void TimePrevBehaveAsExpected(string actual, int prevTimes, string expected)
    {
        Assert.Equal(expected, actual.FromIso().Prev(prevTimes));

    }


    [Theory]
    [InlineData("2017", "2018", IntervalRelation.Before)]
    [InlineData("2017", "2017", IntervalRelation.Equal)]
    [InlineData("2018", "2017", IntervalRelation.After)]
    [InlineData("2017-01", "2017-02", IntervalRelation.Before)]
    [InlineData("2017-01", "2017-01", IntervalRelation.Equal)]
    [InlineData("2017-02", "2017-01", IntervalRelation.After)]
    [InlineData("2017-01-01", "2017-01-02", IntervalRelation.Before)]
    [InlineData("2017-01-01", "2017-01-01", IntervalRelation.Equal)]
    [InlineData("2017-01-02", "2017-01-01", IntervalRelation.After)]
    [InlineData("2017-004", "2017-005", IntervalRelation.Before)]
    [InlineData("2017-005", "2017-005", IntervalRelation.Equal)]
    [InlineData("2017-005", "2017-004", IntervalRelation.After)]
    [InlineData("2017-02-23T10:20", "2017-02-23T11:20", IntervalRelation.Before)]
    [InlineData("2017-02-23T10:20", "2017-02-23T10:20", IntervalRelation.Equal)]
    [InlineData("2017-02-23T11:20", "2017-02-23T10:20", IntervalRelation.After)]
    [InlineData("2017-02-23T10:20", "2017-02-23T10:21", IntervalRelation.Before)]
    [InlineData("2017-02-23T10:21", "2017-02-23T10:20", IntervalRelation.After)]
    [InlineData("2017-02-23T10:20:30", "2017-02-23T10:20:31", IntervalRelation.Before)]
    [InlineData("2017-02-23T10:20:30", "2017-02-23T10:20:30", IntervalRelation.Equal)]
    [InlineData("2017-02-23T10:20:31", "2017-02-23T10:20:30", IntervalRelation.After)]
    //[InlineData("2017-W51", "2017-W52", IntervalRelation.Before)]
    //[InlineData("2017-W52", "2017-W52", IntervalRelation.Equal)]
    //[InlineData("2017-W52", "2017-W51", IntervalRelation.After)]
    public void TimeRelationToOtherTimeBeAsExpected(string leftSideIso, string rightSideIso, IntervalRelation expectingRelation)
    {
        var leftSideTime = leftSideIso.FromIso().ToTime();
        var rightSideTime = rightSideIso.FromIso().ToTime();

        Assert.Equal(expectingRelation, leftSideTime.CompareTo(rightSideTime));
    }
}

public class TimeZoneTests
{
    [Fact]
    public void TimeZone_Conversion_Respects_DST()
    {
        var utcZone = TimeZoneInfo.Utc;
        var nyZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // Windows ID for America/New_York
        // 2023-03-12T06:30:00Z is 1:30 AM EST, just before DST starts in NY
        var t = YearMonthDayTimeHourMinuteSecondTime.New("2023", "03", "12", "01", "30", "00", nyZone);
        var utc = t.WithTimeZone(utcZone);
        Assert.Equal("2023-03-12T06:30:00Z", utc.ToIso());
        // Now convert back to NY time
        var ny = utc.WithTimeZone(nyZone);
        Assert.Equal("2023-03-12T01:30:00-05:00", ny.ToIso());
    }

    [Fact]
    public void TimeZone_Conversion_Handles_DST_Forward()
    {
        var utcZone = TimeZoneInfo.Utc;
        var nyZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        // 2023-03-12T07:30:00Z is 3:30 AM EDT, just after DST starts in NY
        var t = YearMonthDayTimeHourMinuteSecondTime.New("2023", "03", "12", "03", "30", "00", nyZone);
        var utc = t.WithTimeZone(utcZone);
        Assert.Equal("2023-03-12T07:30:00Z", utc.ToIso());
        // Now convert back to NY time
        var ny = utc.WithTimeZone(nyZone);
        Assert.Equal("2023-03-12T03:30:00-04:00", ny.ToIso());
    }
}