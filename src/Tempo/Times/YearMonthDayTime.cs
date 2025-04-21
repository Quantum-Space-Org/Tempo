using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTime : Time
{
    public static YearMonthDayTime New(string year, string month, string day)
    {
        return new YearMonthDayTime(year, month, day);
    }

    private static Time New(YearSequencer year, MonthSequencer month, DaySequencer day)
        => new YearMonthDayTime(year, month, day);

    public YearMonthDayTime(string year, string month, string day) : base(YearSequencer.New(int.Parse(year)))
    {
        Month(month);
        Day(day);
    }
    
    public YearMonthDayTime(YearSequencer year, MonthSequencer month, DaySequencer day) : base(year)
    {
        Month(month.Current().ToString());
        Day(day.Current().ToString());
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
        => $"{_year.ToString()}-{_month.ToString()}-{_day.ToString()}";

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