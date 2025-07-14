using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearDayTime : Time
{
    public static YearDayTime New(YearSequencer year, DaySequencer day, TimeZoneInfo timeZoneInfo = null)
        => new(year, day, timeZoneInfo);

    public static YearDayTime New(string year, string day, TimeZoneInfo timeZoneInfo = null) 
        => new(year, day, timeZoneInfo);

    public YearDayTime(string year, string day, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        SetDay(day);
    }
    
    public YearDayTime(YearSequencer year, DaySequencer day, TimeZoneInfo timeZoneInfo = null) : base(year, timeZoneInfo)
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
        return ToIso();
    }

    public override string ToIso()
    {
        return $"{_year.Current():D4}-{_day.Current():D3}";
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

    public override DateTime ToDateTime()
    {
        return new DateTime(_year.Current(), 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddDays(_day.Current() - 1);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        _day = new DaySequencer(DateTime.IsLeapYear(dateTime.Year) ? 366 : 365, dateTime.DayOfYear);
        this.TimeZoneInfo = zone;
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