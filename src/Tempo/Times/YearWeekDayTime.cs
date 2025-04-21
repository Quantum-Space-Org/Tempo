using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearWeekDayTime : Time
{
    private WeekYearSequencer _currentWeek;
    private DayWeekSequencer _dayWeek;
    private const string Symbol = "W";

    public static YearWeekDayTime New(string year, string week, string day)
        => new(year, week, day);


    public YearWeekDayTime(string year, string week, string day) : base(YearSequencer.New(int.Parse(year)))
    {
        var current = int.Parse(week[1..]);

        SetCurrentWeek(current);

        SetCurrentDyOfWeek(day);
    }

   

    public override string ToIso()
    {
        return $"{_year.ToString()}-{Symbol}{_currentWeek.ToString()}-{_dayWeek.ToString()}";
    }
    

    public override Time Next(int times = 1)
    {
        SetCurrentDyOfWeek(_dayWeek.Next(times).Current().ToString());
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        SetCurrentDyOfWeek(_dayWeek.Prev(times).Current().ToString());
        
        return Clone();
    }

    private void SetCurrentDyOfWeek(string day)
    {
        _dayWeek = new DayWeekSequencer(int.Parse(day));

        _dayWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _currentWeek.Next();

            SetCurrentWeek(sequencer.Current());
        });

        _dayWeek.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _currentWeek.Prev();

            SetCurrentWeek(sequencer.Current());
        });
    }

    private void SetCurrentWeek(int current)
    {
        _currentWeek = new WeekYearSequencer(current);

        _currentWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _year.Next();
            _year = new YearSequencer(sequencer.Current());
        });
    }


    public override string ToString()
    {
        return $"{_year.ToString()}-{_day.ToString()}";
    }

    protected override Time Clone()
    {
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _currentWeek.Current();
        yield return _dayWeek.Current();
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