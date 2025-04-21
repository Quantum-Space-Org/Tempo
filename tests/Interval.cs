namespace Quantum.Tempo.Tests;

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