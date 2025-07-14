namespace Quantum.Tempo.Tests;

public class StringDateToIsoFormatTests
{
    [Theory]
    [InlineData("2013/02/27", "2013-02-27")]
    [InlineData("2013/27/02", "2013-02-27")]
    [InlineData("2013/27/2", "2013-02-27")]
    [InlineData("2013/2/2", "2013-02-02")]

    [InlineData("13/27/02", "2013-02-27")]
    [InlineData("13/02/27", "2013-02-27")]
    [InlineData("13/2/27", "2013-02-27")]

    [InlineData("2017.02.27", "2017-02-27")]
    [InlineData("2017.27.02", "2017-02-27")]
    [InlineData("2017.27.2", "2017-02-27")]
    [InlineData("2017.2.27", "2017-02-27")]
    [InlineData("2017.2.28", "2017-02-28")]
    public void normalizeDate(string actualDate, string expectedIsoFormatted)
    {
        actualDate
            .NormalizeToIsoDateString()
            .Should()
            .Be(expectedIsoFormatted);
    }

    [Theory]
    [InlineData("1402/02/01", "2023-04-21")] // Persian 1402-02-01 is 2023-04-21 Gregorian
    [InlineData("1399-12-30", "2021-03-20")] // Persian leap year
    [InlineData("1400.1.1", "2021-03-21")]
    public void normalizePersianDate(string persianDate, string expectedGregorianIso)
    {
        persianDate
            .NormalizeToPersianIsoDateString()
            .Should()
            .Be(expectedGregorianIso);
    }

    [Theory]
    [InlineData("1445/10/01", "2024-04-10")] // Hijri 1445-10-01 is 2024-04-10 Gregorian
    [InlineData("1440-09-30", "2019-06-03")]
    [InlineData("1442.1.1", "2020-08-20")]
    public void normalizeHijriDate(string hijriDate, string expectedGregorianIso)
    {
        hijriDate
            .NormalizeToHijriIsoDateString()
            .Should()
            .Be(expectedGregorianIso);
    }

    [Theory]
    [InlineData("1404-02-23", "2025-05-13")] // Persian auto-detect
    [InlineData("2025-02-23", "2025-02-23")] // Gregorian auto-detect
    public void autoNormalizeDate(string input, string expected)
    {
        input
            .AutoNormalizeToIsoDateString()
            .Should()
            .Be(expected);
    }

    // Ambiguous year (1400-1600): use explicit normalization for Hijri
    [Theory]
    [InlineData("1445-10-01", "2024-04-09")] // Hijri explicit
    public void explicitHijriNormalize(string input, string expected)
    {
        input
            .NormalizeToHijriIsoDateString()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("2023-04-21", "1402-02-01")] // Persian
    [InlineData("2021-03-20", "1399-12-30")] // Persian leap year
    [InlineData("2021-03-21", "1400-01-01")]
    public void toPersianString(string gregorianIso, string expectedPersian)
    {
        gregorianIso
            .ToPersianString()
            .Should()
            .Be(expectedPersian);
    }

    [Theory]
    [InlineData("2024-04-09", "1445-10-01")] // Hijri (matches .NET HijriCalendar)
    [InlineData("2019-06-03", "1440-09-30")]
    [InlineData("2020-08-19", "1442-01-01")]
    public void toHijriString(string gregorianIso, string expectedHijri)
    {
        gregorianIso
            .ToHijriString()
            .Should()
            .Be(expectedHijri);
    }

    [Theory]
    [InlineData("2023-04-21", 1, "2023-04-22")] // Gregorian next
    [InlineData("1402-02-01", 1, "1402-02-02")] // Persian next
    [InlineData("1445-10-01", 1, "1445-10-02")] // Hijri next (matches .NET HijriCalendar)
    public void nextDate(string input, int times, string expected)
    {
        input.NextDate(times).Should().Be(expected);
    }

    [Theory]
    [InlineData("2023-04-21", 1, "2023-04-20")] // Gregorian prev
    [InlineData("1402-02-01", 1, "1402-01-31")] // Persian prev
    [InlineData("1445-10-01", 1, "1445-09-30")] // Hijri prev (matches .NET HijriCalendar)
    public void prevDate(string input, int times, string expected)
    {
        input.PrevDate(times).Should().Be(expected);
    }

    [Fact]
    public void sequenceGregorian()
    {
        var seq = StringExtensions.Sequence("2023-04-21", "2023-04-23");
        seq.Should().BeEquivalentTo(new[] { "2023-04-21", "2023-04-22", "2023-04-23" });
    }

    [Fact]
    public void sequencePersian()
    {
        var seq = StringExtensions.Sequence("1402-02-01", "1402-02-03");
        seq.Should().BeEquivalentTo(new[] { "1402-02-01", "1402-02-02", "1402-02-03" });
    }

    [Fact]
    public void sequenceHijri()
    {
        var seq = StringExtensions.Sequence("1445-10-01", "1445-10-03");
        seq.Should().BeEquivalentTo(new[] { "1445-10-01", "1445-10-02", "1445-10-03" });
    }
}