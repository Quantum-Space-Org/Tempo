using System;
using Xunit;
using Quantum.Tempo;
using Quantum.Tempo.Times;
using System.Collections.Generic;

namespace Quantum.Tempo.Tests;

public class DurationTests
{
    [Theory]
    [InlineData("P1D", 86400)]
    [InlineData("PT2H30M", 9000)]
    [InlineData("3 days", 259200)]
    [InlineData("2:30:00", 9000)]
    public void Parse_Duration_And_TotalSeconds(string input, long expectedSeconds)
    {
        var duration = Duration.Parse(input);
        Assert.Equal(expectedSeconds, duration.ToTotalSeconds());
    }

    [Theory]
    [InlineData("P1D", "1 day")]
    [InlineData("PT2H30M", "2 hours, 30 minutes")]
    [InlineData("PT0S", "0 seconds")]
    public void ToHumanString_Works(string input, string expected)
    {
        var duration = Duration.Parse(input);
        Assert.Equal(expected, duration.ToHumanString());
    }

    [Theory]
    [InlineData("P1D", "PT2H", "P1DT2H")]
    [InlineData("PT30M", "PT45M", "PT1H15M")]
    [InlineData("3 days", "2:00:00", "P3DT2H")]
    public void Add_Works(string d1, string d2, string expectedIso)
    {
        var result = Duration.Add(d1, d2);
        Assert.Equal(expectedIso, result);
    }

    [Theory]
    [InlineData("P1DT2H", "PT2H", "P1D")]
    [InlineData("PT45M", "PT30M", "PT15M")]
    [InlineData("PT30M", "PT45M", "PT0S")]
    public void Subtract_Works(string d1, string d2, string expectedIso)
    {
        var result = Duration.Subtract(d1, d2);
        Assert.Equal(expectedIso, result);
    }

    [Theory]
    [InlineData("P2D", "P1D", true)]
    [InlineData("PT1H", "PT2H", false)]
    public void IsLongerThan_Works(string d1, string d2, bool expected)
    {
        Assert.Equal(expected, Duration.IsLongerThan(d1, d2));
    }

    [Theory]
    [InlineData("P1D", "P2D", true)]
    [InlineData("PT2H", "PT1H", false)]
    public void IsShorterThan_Works(string d1, string d2, bool expected)
    {
        Assert.Equal(expected, Duration.IsShorterThan(d1, d2));
    }

    [Theory]
    [InlineData("P1D", "P1D", true)]
    [InlineData("PT1H", "PT2H", false)]
    public void Equals_Works(string d1, string d2, bool expected)
    {
        Assert.Equal(expected, Duration.Equals(d1, d2));
    }

    [Theory]
    [InlineData("2023-01-01", "P1D", "2023-01-02")]
    [InlineData("2023-01-01", "PT2H", "2023-01-01")]
    public void AddDuration_Works(string date, string duration, string expected)
    {
        Assert.Equal(expected, date.AddDuration(duration));
    }

    [Theory]
    [InlineData("2023-01-02", "P1D", "2023-01-01")]
    [InlineData("2023-01-01", "PT2H", "2023-01-01")]
    public void SubtractDuration_Works(string date, string duration, string expected)
    {
        Assert.Equal(expected, date.SubtractDuration(duration));
    }

    [Fact]
    public void SequenceByDuration_Works()
    {
        var result = "2023-01-01".SequenceByDuration("2023-01-05", "P2D");
        Assert.Equal(new[] { "2023-01-01", "2023-01-03", "2023-01-05" }, result);
    }
} 