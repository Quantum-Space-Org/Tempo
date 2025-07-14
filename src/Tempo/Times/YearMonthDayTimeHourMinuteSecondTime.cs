using System;
using System.Collections.Generic;
using Quantum.Tempo;

namespace TimeSequencer.Times;

public class YearMonthDayTimeHourMinuteSecondTime : Time
{
    
    public static YearMonthDayTimeHourMinuteSecondTime Year(string year, TimeZoneInfo timeZoneInfo = null)
    {
        return new YearMonthDayTimeHourMinuteSecondTime(year, timeZoneInfo);
    }

    public static YearMonthDayTimeHourMinuteSecondTime New(string year, string month, string day
        , string hour, string minute, string second, TimeZoneInfo timeZoneInfo = null)
    {
        return new(year, month, day, hour, minute , second, timeZoneInfo);
    }

    protected YearMonthDayTimeHourMinuteSecondTime(string year, TimeZoneInfo timeZoneInfo = null)
        : base(new YearSequencer(int.Parse(year)), timeZoneInfo)
    {
    }

    private YearMonthDayTimeHourMinuteSecondTime(string year, 
        string month, string day, string hour, string minute, string second, TimeZoneInfo timeZoneInfo = null)
        : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        Month(month);
        Day(day);
        Hour(hour);
        Minute(minute);
        Second(second);

    }


    public override Time Next(int times = 1)
    {
        SecondNextTimes(times);
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        SecondPrevTimes(times);
        return Clone();
    }

    protected override Time Clone()
    {
        // Deep clone with current values and time zone
        return new YearMonthDayTimeHourMinuteSecondTime(
            GetYear().ToString(),
            GetMonth().ToString(),
            GetDay().ToString(),
            GetHour().ToString(),
            GetMinute().ToString(),
            GetSecond().ToString(),
            this.TimeZoneInfo
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
    


    protected override TimeSequence NestMonth()
        => new(new YearMonthTime(GetYear().ToString(), GetMonth().ToString()), 1);

    protected override TimeSequence NestDay()
        => new(new YearMonthDayTime(GetYear().ToString(), GetMonth().ToString(), GetDay().ToString()), 1);

    public override Time EnclosingImmediate() 
        => new YearMonthDayTimeHourMinuteTime(GetYear().ToString(), GetMonth().ToString(), GetDay().ToString(), _hour.Current().ToString(), _minute.Current().ToString(), this.TimeZoneInfo);

    public override string ToString()
    {
        return ToIso();
    }

    
    public override string ToIso()
    {
        var iso = $"{_year.Current():D4}-{_month.Current():D2}-{_day.Current():D2}T{_hour.Current():D2}:{_minute.Current():D2}:{_second.Current():D2}";
        var offset = TimeZoneInfo.GetUtcOffset(ToDateTime());
        string offsetString;
        if (offset == TimeSpan.Zero)
            offsetString = "Z";
        else
            offsetString = string.Format("{0}{1:00}:{2:00}", offset.TotalMinutes < 0 ? "-" : "+", Math.Abs(offset.Hours), Math.Abs(offset.Minutes));
        return $"{iso}{offsetString}";
    }

    public override DateTime ToDateTime()
    {
        return new DateTime(
            GetYear(),
            GetMonth() ?? 1,
            GetDay() ?? 1,
            GetHour() ?? 0,
            GetMinute() ?? 0,
            GetSecond() ?? 0,
            DateTimeKind.Unspecified
        );
    }

    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        _month = new MonthSequencer(dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        _day = new DaySequencer(DateTime.DaysInMonth(dateTime.Year, dateTime.Month), dateTime.Day);
        _hour = new HourSequencer(dateTime.Hour);
        _minute = new MinuteSequencer(dateTime.Minute);
        _second = new SecondSequencer(dateTime.Second);
        this.TimeZoneInfo = zone;
    }
}