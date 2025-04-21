using System.Globalization;

namespace Quantum.Tempo.Tests;

public class StringifyTests
{
    [Theory]
    [InlineData("2017-02-13")]
    [InlineData("2017-02")]
    [InlineData("2017-02-13T18:09")]
    [InlineData("2017-W05-2")]
    public void isInIsoFormat(string date)
        => Assert.True(date.IsAValidIso8601());

    [Theory]
    [InlineData("2017-02-13", "yyyy-MM-dd")]
    [InlineData("2017-02", "yyyy-MM")]
    [InlineData("2017-02-13T18:09", "yyyy-MM-dd'T'HH:mm")]
    [InlineData("2017-W05-2", "xxxx-'W'ww-e")]
    [InlineData("2017-W05", "xxxx-'W'ww")]
    public void isoPattern(string date, string expectedPattern)
        => Assert.Equal(expectedPattern, date.IsoPattern());


    [Theory(DisplayName = "A yearMonthDayTimeHourMinuteSecond string with yearMonthDayTimeHourMinuteSecond-zone information is parsed into three parts")]
    [InlineData("2017-10-31T20:00", "2017-10-31T20:00")]
    [InlineData("2017-10-31T20:00-05", "2017-10-31T20:00", "-05")]
    [InlineData("2017-10-31T20:00-05:00", "2017-10-31T20:00", "-05:00")]
    [InlineData("2017-10-31T20:00+05:00", "2017-10-31T20:00", "+05:00")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]", "2017-10-31T20:00", "-05:00", "[America/New_York]")]
    [InlineData("2017", "2017")]
    [InlineData("2017T-05:00", "2017", "-05:00")]
    [InlineData("2017T+05:00", "2017", "+05:00")]
    public void timeWithTimeZone(string actualDate, string time, string offset = "", string zone = "")
    {
        var result = actualDate.SplitTime();
        Assert.Equal(time, result.time);
        Assert.Equal(offset, result.offset);
        Assert.Equal(zone, result.zone);
    }

    [Fact(DisplayName = "In cannonical string, least significant place is scale.")]
    public void DateTimeToIso()
    {
        Assert.Equal("2017-01-10",
            new DateTime(2017, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc)
                .ToIso("year month day"));

        Assert.Equal("2017-01",
            new DateTime(2017, 1, 10, 0, 0, 0, 0, new GregorianCalendar())
                .ToIso("year month"));
        Assert.Equal("2017",
            new DateTime(2017, 1, 10, 0, 0, 0, 0, new GregorianCalendar())
                .ToIso("year"));
    }


    [Fact]
    public void fromIsoPattern()
    {
        var dateTime = "2017-01".FromIso().ToDateTime();

        Assert.Equal(2017, dateTime.Year);
        Assert.Equal(01, dateTime.Month);
        Assert.Equal(01, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
        Assert.Equal(0, dateTime.Minute);
        Assert.Equal(DateTimeKind.Local, dateTime.Kind);

        dateTime = "2017-01-10".FromIso().ToDateTime();

        Assert.Equal(2017, dateTime.Year);
        Assert.Equal(01, dateTime.Month);
        Assert.Equal(10, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
        Assert.Equal(0, dateTime.Minute);
        Assert.Equal(DateTimeKind.Local, dateTime.Kind);
    }

    [Fact(DisplayName = "A string with no timezone info is represented as LocalDateTime")]
    public void name()
    {
        // from iso to date yearMonthDayTimeHourMinuteSecond

        var dateTime = "2017-11-05T04:35".FromIso().ToDateTime();

        Assert.Equal(2017, dateTime.Year);
        Assert.Equal(11, dateTime.Month);
        Assert.Equal(5, dateTime.Day);
        Assert.Equal(4, dateTime.Hour);
        Assert.Equal(35, dateTime.Minute);

        Assert.Equal(DateTimeKind.Local, dateTime.Kind);

    }

    [Fact(DisplayName = "Relation bounded intervals can be represented as ISO")]
    public void fromIsoInterval()
    {
        var dateTimes = "2017-05-15/2017-05-17"

            .FromIso()
            .ToTimeInterval();

        Assert.Equal(2017, dateTimes.Start?.GetYear());
        Assert.Equal(05, dateTimes.Start?.GetMonth());
        Assert.Equal(15, dateTimes.Start?.GetDay());

        Assert.Equal(2017, dateTimes.Finish?.GetYear());
        Assert.Equal(05, dateTimes.Finish?.GetMonth());
        Assert.Equal(17, dateTimes.Finish?.GetDay());

    }



    [Fact(DisplayName = "When a specific date is in range of an interval then the relation should be Contains")]
    public void WhenASpecificDateIsInRangeOfAnIntervalThenTheRelationShouldBeContains()
    {
        const IntervalRelation expectingIntervalRelation = IntervalRelation.Contains;
        const string datesRange = "2021-10-01/2021-12-01";
        const string date = "2021-11-01";

        var actualRelation = datesRange.FromIso().ToTimeInterval().Relation(date.FromIso().ToTime());
        actualRelation.Should().Be(expectingIntervalRelation);
    }

    [Fact(DisplayName = "When a specific date is before the range of an interval then the relation should be Before")]
    public void WhenASpecificDateIsBeforeTheRangeOfAnIntervalThenTheRelationShouldBeBefore()
    {
        var expectingIntervalRelation = IntervalRelation.Before;
        var datesRange = "2021-10-01/2021-12-01";
        var date = "2021-09-01";

        var actualRelation = datesRange.FromIso().ToTimeInterval().Relation(date.FromIso().ToTime());
        actualRelation.Should().Be(expectingIntervalRelation);
    }

    [Fact(DisplayName = "Sequences can be converted both ways")]
    public void Sequences_can_be_converted_both_ways()
    {
        Assert.Equal("2017-05-15/2017-05-17", (new DateTime(2017, 05, 15), new DateTime(2017, 05, 17))
            .ToIso());

    }

    [Fact(DisplayName = "Parsing can infer common ISO 8601 date-yearMonthDayTimeHourMinuteSecond or interval formats.")]
    public void intervalWithJustOneSide()
    {
        var relationBoundedInterval = "2017-05-15".FromIso().ToTimeInterval();
        Assert.Equal(null, relationBoundedInterval.Finish);

        relationBoundedInterval = "2017-05-15/2017-05-17".FromIso().ToTimeInterval();

        Assert.NotEqual(null, relationBoundedInterval.Start);
        Assert.NotEqual(null, relationBoundedInterval.Finish);
    }

    [Fact(DisplayName =
        "ISO 8601 doesn't seem to have a one-sided interval format. We use '-' to indicate a missing bound.")]
    public void fromIsoIntervalWithOneSide()
    {
        var relationBoundedInterval = "2017-05-15/-"
            .FromIso()
            .ToTimeInterval();

        Assert.Equal(2017, relationBoundedInterval.Start?.GetYear());
        Assert.Equal(05, relationBoundedInterval.Start?.GetMonth());
        Assert.Equal(15, relationBoundedInterval.Start?.GetDay());
        Assert.Equal(null, relationBoundedInterval.Finish);

        var boundedInterval = "-/2017-05-17".FromIso()
            .ToTimeInterval();

        Assert.Equal(null, boundedInterval.Start);
        Assert.Equal(2017, boundedInterval.Finish?.GetYear());
        Assert.Equal(05, boundedInterval.Finish?.GetMonth());
        Assert.Equal(17, boundedInterval.Finish?.GetDay());
    }


    [Fact(DisplayName = "Relation bounded intervals can be represented as ISO")]
    public void fromDateTimeToIsoInterval()
    {
        var start = new DateTime(2017, 05, 15, 0, 0, 0);
        var finish = new DateTime(2017, 05, 17, 0, 0, 0);

        var result = (start, finish).ToIsoInterval();

        Assert.Equal("2017-05-15/2017-05-17", result);
    }


    [Fact(DisplayName = "A string with an offset is represented as DateTime with offset")]
    public void A_string_with_an_offset_is_represented_as_DateTime_with_offset()
    {
        var dateTime = "2017-11-05T04:35-03:15"
            .FromIso()
            .ToDateTime();

        Assert.Equal(2017, dateTime.Year);
        Assert.Equal(11, dateTime.Month);
        Assert.Equal(05, dateTime.Day);
        Assert.Equal(4, dateTime.Hour);
        Assert.Equal(35, dateTime.Minute);

        Assert.Equal(DateTimeKind.Local, dateTime.Kind);
    }

    [Theory(DisplayName = "The ends of a relation-bounded interval are separated by a slash.")]
    [InlineData("2017-10-31T20:00/2017-10-31T20:30")]
    [InlineData("2017-10-31T20:00")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]/2017-10-31T21:05-06:00[America/Chicago]")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]/-")]
    [InlineData("-/2017-10-31T20:00-05:00[America/New_York]")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York] / -")]
    public void The_ends_of_a_relation_bounded_interval_are_separated_by_a_slash(string value)
    {
        value
            .FromIso()
            .ToTimeInterval();

        "2017-10-31T20:00"
            .FromIso()
            .ToTimeInterval();

    }

    [Theory]
    [InlineData("2017-10-31T20:00")]
    [InlineData("2017-10-31T20")]
    [InlineData("2017-W12")]
    [InlineData("2017-W12-2")]
    [InlineData("2017-10-31T20:00-05")]
    [InlineData("2017-10-31T20:00-05:00")]
    [InlineData("2017-10-31T20:00+05:00")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]")]
    [InlineData("2017")]
    [InlineData("2017T-05:00")]
    [InlineData("2017T+05:00")]
    public void isoToTime(string value)
    {
        var time = value.FromIso().ToTime();



    }

    [Theory]
    [InlineData("2017", 3, "2020")]
    [InlineData("2017-W51", 1, "2017-W52")]
    [InlineData("2017-W52", 3, "2018-W03")]
    [InlineData("2017-W01", 52, "2018-W01")]
    [InlineData("2017-02-23", 5, "2017-02-28")]
    [InlineData("2017-12-01", 31, "2018-01-01")]
    [InlineData("2017-02-23", 6, "2017-03-01")]
    [InlineData("2017-02-23T20:30", 6, "2017-02-23T20:36")]
    [InlineData("2017-02-23T20:50", 10, "2017-02-23T21:00")]
    [InlineData("2017-360", 5, "2017-365")]
    [InlineData("2017-360", 10, "2018-005")]
    [InlineData("2017-W52-1", 2, "2017-W52-3")]
    [InlineData("2017-W52-1", 7, "2018-W01-1")]
    [InlineData("2017-W50-7", 14, "2017-W52-7")]

    public void quantum_time_next_should_behaves_ex_expected(string actual, int nextTimes, string expected)
        => Assert.Equal(expected, actual.FromIso().ToTime().Next(nextTimes).ToIso());

    [Theory]
    [InlineData("2017", 3)]
    [InlineData("2017-W51", 1)]
    [InlineData("2017-W52", 3)]
    [InlineData("2017-W01", 52)]
    [InlineData("2017-02-23", 5)]
    [InlineData("2017-12-01", 31)]
    [InlineData("2017-02-23", 6)]
    [InlineData("2017-02-23T20:30", 6)]
    [InlineData("2017-02-23T20:50", 10)]
    [InlineData("2017-360", 5)]
    [InlineData("2017-360", 10)]
    [InlineData("2017-W52-1", 2)]
    [InlineData("2017-W52-1", 7)]
    [InlineData("2017-W50-7", 14)]

    public void canonicalCoutingOperations(string actual, int nextTimes)
    {
        Assert.Equal(actual.FromIso().ToTime().Later(nextTimes).ToIso(),
            actual.FromIso().ToTime().Next(nextTimes).ToIso());

        Assert.Equal(actual.FromIso().ToTime().Before(nextTimes).ToIso(),
            actual.FromIso().ToTime().Prev(nextTimes).ToIso());


        Assert.Equal(actual.FromIso().Later(nextTimes).ToIso(), actual.FromIso().Next(nextTimes).ToIso());

        Assert.Equal(actual.FromIso().Before(nextTimes).ToIso(), actual.FromIso().Prev(nextTimes).ToIso());
    }

    [Fact]
    public void accumolativeTime()
    {
        //Assert.True(false);
        var actual = "2017-02-23";

        Assert.Equal(actual.FromIso().Next(2),
            actual.FromIso().Next().FromIso().Next().ToIso());
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
    public void TimePrevBehaveAsExpected(string actual, int prevTimes, string expected)
    {
        Assert.Equal(expected, actual.FromIso().ToTime().Prev(prevTimes).ToIso());
        //Assert.Equal(expected, actual.FromIso().NextIso(prevTimes).ToIso());
    }
    [Theory]
    [InlineData("2017-10-31T20:00")]
    [InlineData("2017-10-31T20")]
    [InlineData("2017-W12")]
    [InlineData("2017-W12-2")]
    [InlineData("2017-10-31T20:00-05")]
    [InlineData("2017-10-31T20:00-05:00")]
    [InlineData("2017-10-31T20:00+05:00")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]")]
    [InlineData("2017")]
    [InlineData("2017T-05:00")]
    [InlineData("2017T+05:00")]
    public void timeToIso(string value)
    {
        var time = value.FromIso().ToTime();

        Assert.Equal(value, time.ToIso());
    }

    [Theory]
    [InlineData("2017-10-31T20:00")]
    [InlineData("2017-10-31T20:00-05")]
    [InlineData("2017-10-31T20:00-05:00")]
    [InlineData("2017-10-31T20:00+05:00")]
    [InlineData("2017-10-31T20:00-05:00[America/New_York]")]
    [InlineData("2017")]
    [InlineData("2017T-05:00")]
    [InlineData("2017T+05:00")]
    public void isoToTimeInerval(string value)
    {
        var time = value
            .FromIso()
            .ToTimeInterval();
    }


    [Theory]
    [InlineData("2017")]
    [InlineData("2017-W51")]
    [InlineData("2017-W52")]
    [InlineData("2017-W01")]
    [InlineData("2017-02-23")]
    [InlineData("2017-12-01")]
    [InlineData("2017-02-23T20:30")]
    [InlineData("2017-02-23T20:50")]
    [InlineData("2017-360")]
    [InlineData("2017-W52-1")]
    [InlineData("2017-W50-7")]

    public void equality(string actual) => Assert.Equal(actual.FromIso().ToTime(), actual.FromIso().ToTime());
}