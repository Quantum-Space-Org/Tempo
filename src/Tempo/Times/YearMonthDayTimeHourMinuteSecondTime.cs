using System;
using System.Collections.Generic;
using Quantum.Tempo;

namespace TimeSequencer.Times;

public class YearMonthDayTimeHourMinuteSecondTime : Time
{
    
    public static YearMonthDayTimeHourMinuteSecondTime Year(string year)
    {
        return new YearMonthDayTimeHourMinuteSecondTime(year);
    }

    public static YearMonthDayTimeHourMinuteSecondTime New(string year, string month, string day
        , string hour, string minute, string second)
    {
        return new(year, month, day, hour, minute , second);
    }

    protected YearMonthDayTimeHourMinuteSecondTime(string year)
        : base(new YearSequencer(int.Parse(year)))
    {
    }

    private YearMonthDayTimeHourMinuteSecondTime(string year, 
        string month, string day, string hour, string minute, string second)
        : base(YearSequencer.New(int.Parse(year)))
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
        return this;
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
        => new YearMonthDayTimeHourMinuteTime(GetYear().ToString()
            , GetMonth().ToString()
            , GetDay().ToString()
            , _hour.Current().ToString()
            , _minute.Current().ToString());

    public override string ToString()
    {
        return ToIso();
    }

    
    public override string ToIso()
    {
        var iso = $"{_year.ToString()}-{_month.ToString()}-{_day.ToString()}T{_hour.ToString()}:{_minute.ToString()}:{_second.ToString()}";

        if (_offsetValue is not "" && _timezone is not "")
            iso = $"{iso}{_offsetValue}{_timezone}";
        else if (_offsetValue is not "")
            iso = $"{iso}{_offsetValue}";
        else if (_timezone is not "")
            iso = $"{iso}{_timezone}";
        return iso;
    }
}