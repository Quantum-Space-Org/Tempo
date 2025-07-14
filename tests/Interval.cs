using System;
using Xunit;
using Quantum.Tempo;
using System.Collections.Generic;

public class IntervalTests
{
    [Fact(DisplayName = "A relation-bounded interval defines a sequence of CountableTimes")]
    public void A_relation_bounded_interval_defines_a_sequence_of_TimeSequence()
    {
        const string starts = "2016-12-31";
        const string finishes = "2017-01-02";

        var fromIso = (starts, finishes).FromIso();

        fromIso.Should()
            .BeEquivalentTo($"{starts}/{finishes}");

        fromIso.Sequences().Should().BeEquivalentTo(new[] { "2016-12-31", "2017-01-01", "2017-01-02" });
    }

    [Fact(DisplayName = "A relation-bounded interval defines a sequence of CountableTimes")]
    public void reverse_sequence()
    {
        const string starts = "2016-12-31";
        const string finishes = "2017-01-02";

        var interval = (starts, finishes).FromIso();

        interval.Should()
            .BeEquivalentTo($"{starts}/{finishes}");

        interval
            .ReverseSequence()
            .Should()
            .BeEquivalentTo(new[] { "2017-01-02", "2017-01-01", "2016-12-31" });
        
        interval
            .ReverseSequence()
            .Nth(1)
            .Should()
            .BeEquivalentTo("2017-01-02");
        
        interval
            .ReverseSequence()
            .Start()
            .Should()
            .BeEquivalentTo("2017-01-02");

        interval
            .ReverseSequence()
            .Nth(3)
            .Should()
            .BeEquivalentTo("2016-12-31");
        
        interval
            .ReverseSequence()
            .Finish()
            .Should()
            .BeEquivalentTo("2016-12-31");
    }


    [Fact(DisplayName = "A relation-bounded interval defines a sequence of CountableTimes")]
    public void take_sequence()
    {
        const string starts = "2016-12-31";
        const string finishes = "2017-01-02";

        var interval = (starts, finishes).FromIso();

        interval
            .Sequences()
            .Take(2)
            .Should()
            .BeEquivalentTo(new[] { "2016-12-31", "2017-01-01" });

        interval
            .ReverseSequence()
            .Take(2)
            .Should()
            .BeEquivalentTo(new[] { "2017-01-02", "2017-01-01" });
    }

    [Fact(DisplayName = "A relation-bounded interval defines a sequence of CountableTimes")]
    public void take_sequenced()
    {
        const string bounded_interval= "2016-12-31 / 2017-01-02";

        var interval = bounded_interval.FromIso().ToTimeString();

        interval
            .Sequence()
            .Take(2)
            .Should()
            .BeEquivalentTo(new[] { "2016-12-31", "2017-01-01" });

        interval
            .ReverseSequence()
            .Take(2)
            .Should()
            .BeEquivalentTo(new[] { "2017-01-02", "2017-01-01" });
    }

    [Fact(DisplayName = "Boundary relations must be consistent for all this to work!")]
    public async Task Boundary_relations_must_be_consistent()
    {
        //(consistent ? (from - iso "2016/2018")) => truthy
        //    (consistent ? (from - iso "2019/2016")) => falsey
        //    (consistent ? (from - iso "2016/2017-06")) => truthy
        //    (consistent ? (from - iso "2016/2016-06")) => falsey
        //    (consistent ? (from - iso "2017/-")) => truthy
        //    (consistent ? (from - iso "-/2017")) => truthy)

        "2016/2018".FromIso().Guard().Should().BeTrue();
        
        "2017/-".FromIso().Guard().Should().BeTrue();
        "-/2017".FromIso().Guard().Should().BeTrue();

        "2019 / 2016".FromIso().Guard().Should().BeFalse();
        "2016/2016-06".FromIso().Guard().Should().BeFalse();

    }

    [Theory]
    [InlineData("2024-01-01/2024-01-10", "2024-01-01", "2024-01-10")]
    [InlineData("/2024-01-10", null, "2024-01-10")]
    [InlineData("2024-01-01/", "2024-01-01", null)]
    [InlineData("2024-01-01", "2024-01-01", "2024-01-01")]
    public void Parse_And_ToIntervalString_Works(string input, string expectedStart, string expectedFinish)
    {
        var interval = Quantum.Tempo.Interval.Parse(input);
        Assert.Equal(expectedStart, interval.Start?.ToIso());
        Assert.Equal(expectedFinish, interval.Finish?.ToIso());
        // ToIntervalString roundtrip
        if (expectedStart != null && expectedFinish != null)
            Assert.Equal($"{expectedStart}/{expectedFinish}", interval.ToIntervalString());
    }

    [Theory]
    [InlineData("2024-01-01/2024-01-10", "2024-01-05/2024-01-15", "2024-01-01/2024-01-15")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-11/2024-01-20", null)]
    public void Union_Works(string i1, string i2, string expected)
    {
        Assert.Equal(expected, Quantum.Tempo.Interval.Union(i1, i2));
    }

    [Theory]
    [InlineData("2024-01-01/2024-01-10", "2024-01-05/2024-01-15", "2024-01-05/2024-01-10")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-11/2024-01-20", null)]
    public void Intersection_Works(string i1, string i2, string expected)
    {
        Assert.Equal(expected, Quantum.Tempo.Interval.Intersection(i1, i2));
    }

    [Theory]
    [InlineData("2024-01-01/2024-01-10", "2024-01-05/2024-01-08", new[] { "2024-01-01/2024-01-05", "2024-01-08/2024-01-10" })]
    [InlineData("2024-01-01/2024-01-10", "2024-01-11/2024-01-20", new[] { "2024-01-01/2024-01-10" })]
    public void Difference_Works(string i1, string i2, string[] expected)
    {
        var result = Quantum.Tempo.Interval.Difference(i1, i2);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2024-01-01/2024-01-10", "P9D")]
    [InlineData("2024-01-01/2024-01-01", "PT0S")]
    public void Duration_Works(string interval, string expectedDuration)
    {
        Assert.Equal(expectedDuration, Quantum.Tempo.Interval.Duration(interval));
    }

    [Theory]
    [InlineData("2024-01-01/2024-01-05", "2024-01-06/2024-01-10", "Before")]
    [InlineData("2024-01-06/2024-01-10", "2024-01-01/2024-01-05", "After")]
    [InlineData("2024-01-01/2024-01-05", "2024-01-05/2024-01-10", "Meets")]
    [InlineData("2024-01-05/2024-01-10", "2024-01-01/2024-01-05", "MetBy")]
    [InlineData("2024-01-01/2024-01-05", "2024-01-01/2024-01-10", "Starts")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-01/2024-01-05", "StartedBy")]
    [InlineData("2024-01-05/2024-01-10", "2024-01-01/2024-01-10", "Finishes")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-05/2024-01-10", "FinishedBy")]
    [InlineData("2024-01-03/2024-01-07", "2024-01-01/2024-01-10", "During")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-03/2024-01-07", "Contains")]
    [InlineData("2024-01-01/2024-01-07", "2024-01-05/2024-01-10", "Overlaps")]
    [InlineData("2024-01-05/2024-01-10", "2024-01-01/2024-01-07", "OverlappedBy")]
    [InlineData("2024-01-01/2024-01-10", "2024-01-01/2024-01-10", "Equal")]
    public void Relation_Works(string i1, string i2, string expected)
    {
        Assert.Equal(expected, Quantum.Tempo.Interval.Relation(i1, i2));
    }
}



public static class TimeIntervalExtensions
{
    public static string FromIso(this (string starts, string finishes) boundedIntervals)
    {
        return $"{boundedIntervals.starts}/{boundedIntervals.finishes}";
    }

    public static string[] Sequences(this string interval)
    {
        return interval.FromIso().Sequences();

    }

    public static string[] ReverseSequence(this string interval)
    {
        return interval.FromIso().ReverseSequences();
    }

    public static string[] Take(this string interval)
    {
        return interval.FromIso().ReverseSequences();
    }
}