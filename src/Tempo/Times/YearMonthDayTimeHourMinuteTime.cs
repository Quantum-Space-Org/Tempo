using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthDayTimeHourMinuteTime : Time
{
    public static YearMonthDayTimeHourMinuteTime New(string year, string month, string day
    , string hour, string minute)
    {
        return new(year, month, day, hour, minute);
    }

    public YearMonthDayTimeHourMinuteTime(string year, string month, string day
        , string hour, string minute) : base(YearSequencer.New(int.Parse(year)))
    {
        Month(month);
        Day(day);
        Hour(hour);
        Minute(minute);
    }
    
    public override string ToIso()
    {
        //2017 - 10 - 31T20: 00 - 05
        var iso = $"{_year.ToString()}-{_month.ToString()}-{_day.ToString()}T{_hour.ToString()}:{_minute.ToString()}";

        if (_offsetValue is not "" && _timezone is not "")
            iso = $"{iso}{_offsetValue}{_timezone}";
       else if (_offsetValue is not "")
            iso = $"{iso}{_offsetValue}";
        else if (_timezone is not "")
            iso = $"{iso}{_timezone}";
        return iso;
    }


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

    public override string ToString()
    {
        return ToIso();
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