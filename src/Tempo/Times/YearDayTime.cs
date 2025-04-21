using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearDayTime : Time
{
    public static YearDayTime New(YearSequencer year, DaySequencer day)
        => new(year, day);

    public static YearDayTime New(string year, string day) 
        => new(year, day);

    public YearDayTime(string year, string day) : base(YearSequencer.New(int.Parse(year)))
    {
        SetDay(day);
    }
    
    public YearDayTime(YearSequencer year, DaySequencer day) : base(year)
    {
        SetDay(day.Current().ToString());
    }

   
    public override Time Next(int times = 1)
    {
        SetDay(_day.Next(times).Current().ToString());
        
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        SetDay(_day.Prev(times).Current().ToString());
        return Clone();
    }

    public override string ToString()
    {
        return $"{_year.ToString()}-{_day.ToString()}";
    }

    public override string ToIso()
    {
        return ToString();
    }

    protected override Time Clone()
    {
        return YearDayTime.New(this._year, this._day);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _day.Current();
    }

    public override Time EndOfMonth() 
        => throw new EndOfMonthOnYearTimeException();

    
    protected override TimeSequence NestMonth()
    {
        throw new System.NotImplementedException();
    }

    protected override TimeSequence NestDay() => new(this, 1);
   

    public override Time EnclosingImmediate()
    {
        throw new System.NotImplementedException();
    }

    private void SetDay(string day)
    {
        _day = new DaySequencer(base.IsLeapYear() ? 366 : 365, int.Parse(day));

        
        _day.SetOnReachAtMaxPosition( () =>
        {
            var sequencer = _year.Next();
            _year = new YearSequencer(sequencer.Current());
        });


        _day.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _year.Prev();
            _year = new YearSequencer(sequencer.Current());
        });
    }

}