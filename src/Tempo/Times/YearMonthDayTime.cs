using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTime : Time
{
    public static YearMonthDayTime New(string year, string month, string day, TimeZoneInfo timeZoneInfo = null)
    {
        return new YearMonthDayTime(year, month, day, timeZoneInfo);
    }
    private static Time New(YearSequencer year, MonthSequencer month, DaySequencer day, TimeZoneInfo timeZoneInfo = null)
        => new YearMonthDayTime(year, month, day, timeZoneInfo);
    public YearMonthDayTime(string year, string month, string day, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        Month(month);
        Day(day);
    }
    public YearMonthDayTime(YearSequencer year, MonthSequencer month, DaySequencer day, TimeZoneInfo timeZoneInfo = null) : base(year, timeZoneInfo)
    {
        Month(month.Current().ToString());
        Day(day.Current().ToString());
    }
    public override DateTime ToDateTime()
    {
        return new DateTime(_year.Current(), _month.Current(), _day.Current(), 0, 0, 0, DateTimeKind.Unspecified);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        _month = new MonthSequencer(12, dateTime.Month);
        _day = new DaySequencer(DateTime.DaysInMonth(dateTime.Year, dateTime.Month), dateTime.Day);
        this.TimeZoneInfo = zone;
    }


    public override Time Next(int times = 1)
    {
        DayNextTimes(times);
        return Clone();
    }



    public override Time Prev(int times = 1)
    {
        DayPrevTimes(times);
        return Clone();
    }

    public override string ToString()
        => ToIso();

    public override string ToIso()
        => $"{_year.Current():D4}-{_month.Current():D2}-{_day.Current():D2}";

    protected override Time Clone() => New(this._year, this._month, this._day);


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _month.Current();
        yield return _day.Current();
    }

    
    protected override TimeSequence NestMonth()
        => new(new YearMonthTime(this._year.Current().ToString(), this._month.Current().ToString()), 1);

    protected override TimeSequence NestDay()
        => new(this, 1);

    public override Time EnclosingImmediate() 
        => YearMonthTime.New(GetYear().ToString(), GetMonth().ToString());
}