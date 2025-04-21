using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTimeHourTime : Time
{
    public static YearMonthDayTimeHourTime New(string year, string month, string day
    , string hour)
    {
        return new(year, month, day, hour);
    }

    public YearMonthDayTimeHourTime(string year, string month, string day
        , string hour) : base(YearSequencer.New(int.Parse(year)))
    {
        Month(month);
        Day(day);
        Hour(hour);
    }


    public override Time Next(int times = 1)
    {
        HourNextTimes(times);
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        HourPrevTimes(times);
        return Clone();
    }


    protected override Time Clone()
    {
        return this;
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GetYear();
        yield return GetMonth();
        yield return GetDay();
        yield return _hour.Current();
    }


    protected override TimeSequence NestDay()
        => new(new YearMonthDayTime(GetYear().ToString(), GetMonth().ToString(), GetDay().ToString()),1);

    public override Time EnclosingImmediate()
        => new YearMonthDayTime(GetYear().ToString()
            , GetMonth().ToString()
            , GetDay().ToString());

    protected override TimeSequence NestMonth()
        => new(new YearMonthTime(GetYear().ToString(), GetMonth().ToString()), 1);

    public override string ToString()
        => ToIso();

    public override string ToIso() =>
        $"{_year.ToString()}-{_month.ToString()}-{_day.ToString()}T{_hour.ToString()}";

}