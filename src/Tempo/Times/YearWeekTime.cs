using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearWeekTime : Time
{
    private WeekYearSequencer _currentWeek;
    private const string Symbol = "W";

    public static YearWeekTime New(string year, string week) => new(year, week);


    public YearWeekTime(string year, string week) : base(YearSequencer.New(int.Parse(year)))
    {
        var current = int.Parse(week[1..]);

        _currentWeek = new WeekYearSequencer(current);


        _currentWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _year.Next();
            _year = new YearSequencer(sequencer.Current());
        });

        _currentWeek.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _year.Prev();
            _year = new YearSequencer(sequencer.Current());
        });
    }
    
    

    public override string ToIso()
    {
        return $"{_year.ToString()}-{Symbol}{_currentWeek.ToString()}";
    }

    public override Time Next(int times = 1)
    {
        _currentWeek = WeekYearSequencer.New(_currentWeek.Next(times));
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        _currentWeek = WeekYearSequencer.New(_currentWeek.Prev(times));
        return Clone();
    }

    public override string ToString()
    {
        return ToIso();
    }

    protected override Time Clone()
    {
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _currentWeek.Current();
    }
    
    protected override TimeSequence NestMonth()
    {
        throw new System.NotImplementedException();
    }

    protected override TimeSequence NestDay()
    {
        throw new System.NotImplementedException();
    }

    public override Time EnclosingImmediate()
    {
        throw new System.NotImplementedException();
    }
}