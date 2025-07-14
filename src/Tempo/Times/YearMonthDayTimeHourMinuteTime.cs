using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTimeHourMinuteTime : Time
{
    public static YearMonthDayTimeHourMinuteTime New(string year, string month, string day, string hour, string minute, TimeZoneInfo timeZoneInfo = null)
    {
        return new(year, month, day, hour, minute, timeZoneInfo);
    }
    public YearMonthDayTimeHourMinuteTime(string year, string month, string day, string hour, string minute, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        Month(month);
        Day(day);
        Hour(hour);
        Minute(minute);
    }
    public override DateTime ToDateTime()
    {
        return new DateTime(_year.Current(), _month.Current(), _day.Current(), _hour.Current(), _minute.Current(), 0, DateTimeKind.Unspecified);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        _month = new MonthSequencer(12, dateTime.Month);
        _day = new DaySequencer(DateTime.DaysInMonth(dateTime.Year, dateTime.Month), dateTime.Day);
        _hour = new HourSequencer(dateTime.Hour);
        _minute = new MinuteSequencer(dateTime.Minute);
        this.TimeZoneInfo = zone;
    }
    
    public override string ToIso()
    {
        var iso = $"{_year.Current():D4}-{_month.Current():D2}-{_day.Current():D2}T{_hour.Current():D2}:{_minute.Current():D2}";
        if (_offsetValue is not "" && _timezone is not "")
            iso = $"{iso}{_offsetValue}{_timezone}";
        else if (_offsetValue is not "")
            iso = $"{iso}{_offsetValue}";
        else if (_timezone is not "")
            iso = $"{iso}{_timezone}";
        return iso;
    }
    public override string ToString()
        => ToIso();


    public override Time Next(int times = 1)
    {
        MinuteNextTimes(times);
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
       MinutePrevTimes(times);
        return Clone();
    }

    protected override Time Clone()
    {
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _month.Current();
        yield return _day.Current();
        yield return _hour.Current();
        yield return _minute.Current();
    }
    

    protected override TimeSequence NestMonth()
        => new(new YearMonthTime(GetYear().ToString(), GetMonth().ToString()), 1);

    protected override TimeSequence NestDay()
        => new (new YearMonthDayTime(GetYear().ToString(), GetMonth().ToString(), GetDay().ToString()),1);

    public override Time EnclosingImmediate()
        => new YearMonthDayTimeHourTime(GetYear().ToString()
            , GetMonth().ToString()
            , GetDay().ToString()
            , _hour.Current().ToString());
}