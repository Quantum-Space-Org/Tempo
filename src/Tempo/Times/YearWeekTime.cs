using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearWeekTime : Time
{
    private WeekYearSequencer _currentWeek;
    private const string Symbol = "W";

    public static YearWeekTime New(string year, string week, TimeZoneInfo timeZoneInfo = null) => new(year, week, timeZoneInfo);


    public YearWeekTime(string year, string week, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        var current = int.Parse(week[1..]);

        _currentWeek = new WeekYearSequencer(current);


        _currentWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _year.Next();
            _year = new YearSequencer(sequencer.Current());
        });

        _currentWeek.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _year.Prev();
            _year = new YearSequencer(sequencer.Current());
        });
    }
    
    

    public override string ToIso()
    {
        return $"{_year.ToString()}-{Symbol}{_currentWeek.ToString()}";
    }

    public override Time Next(int times = 1)
    {
        _currentWeek = WeekYearSequencer.New(_currentWeek.Next(times));
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        _currentWeek = WeekYearSequencer.New(_currentWeek.Prev(times));
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
        yield return _currentWeek.Current();
    }
    
    protected override TimeSequence NestMonth()
    {
        throw new System.NotImplementedException();
    }

    protected override TimeSequence NestDay()
    {
        throw new System.NotImplementedException();
    }

    public override Time EnclosingImmediate()
    {
        throw new System.NotImplementedException();
    }

    public override DateTime ToDateTime()
    {
        // ISO week to date: get the first day of the year, add (week-1)*7 days
        var jan1 = new DateTime(_year.Current(), 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return jan1.AddDays((_currentWeek.Current() - 1) * 7);
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        var week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dateTime, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        _currentWeek = new WeekYearSequencer(week);
        this.TimeZoneInfo = zone;
    }
}