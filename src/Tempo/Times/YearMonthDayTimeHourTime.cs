using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTimeHourTime : Time
{
    public static YearMonthDayTimeHourTime New(string year, string month, string day, string hour, TimeZoneInfo timeZoneInfo = null)
    {
        return new(year, month, day, hour, timeZoneInfo);
    }

    public YearMonthDayTimeHourTime(string year, string month, string day, string hour, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
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

    public override DateTime ToDateTime()
    {
        return new DateTime(_year.Current(), _month.Current(), _day.Current(), _hour.Current(), 0, 0, DateTimeKind.Unspecified);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        _month = new MonthSequencer(12, dateTime.Month);
        _day = new DaySequencer(DateTime.DaysInMonth(dateTime.Year, dateTime.Month), dateTime.Day);
        _hour = new HourSequencer(dateTime.Hour);
        this.TimeZoneInfo = zone;
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
        $"{_year.Current():D4}-{_month.Current():D2}-{_day.Current():D2}T{_hour.Current():D2}";

}