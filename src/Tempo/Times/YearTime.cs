using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearTime : Time
{
    public static YearTime New(string year, TimeZoneInfo timeZoneInfo = null) => new(year, timeZoneInfo);
    public YearTime(string year, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo) {}

    public override DateTime ToDateTime()
    {
        return new DateTime(_year.Current(), 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        this.TimeZoneInfo = zone;
    }


    public override string ToIso()
    {
        var iso = $"{_year.ToString()}";
        if (_offsetValue is not null)
            iso = $"{iso}T{_offsetValue}";

        return iso;
    }

    public override Time Next(int times = 1)
    {
        YearNextTimes(times);
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        YearPrevTimes(times);
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

    protected override TimeSequence NestDay() 
        => new(new YearDayTime(this._year.Current().ToString(), 1.ToString()), this.IsLeapYear() ? 366 : 365);

    public override Time EnclosingImmediate() 
        => this;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
    }

    public override Time EndOfMonth() 
        => throw new EndOfMonthOnYearTimeException();

    
    protected override TimeSequence NestMonth() =>
        new(new YearMonthTime(this._year.Current().ToString(), 1.ToString()), 12);

    public override Time EnclosingMonth()
    {
        throw new EnclosingMonthOfYearTimeException(this.GetYear());
    }
}

public class EnclosingMonthOfYearTimeException : Exception
{
    public int Year { get; }

    public EnclosingMonthOfYearTimeException(int year)
    {
        Year = year;
    }
}