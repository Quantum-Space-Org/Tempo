using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearWeekDayTime : Time
{
    private WeekYearSequencer _currentWeek;
    private DayWeekSequencer _dayWeek;
    private const string Symbol = "W";

    public static YearWeekDayTime New(string year, string week, string day, TimeZoneInfo timeZoneInfo = null)
        => new(year, week, day, timeZoneInfo);


    public YearWeekDayTime(string year, string week, string day, TimeZoneInfo timeZoneInfo = null) : base(YearSequencer.New(int.Parse(year)), timeZoneInfo)
    {
        var current = int.Parse(week[1..]);

        SetCurrentWeek(current);

        SetCurrentDyOfWeek(day);
    }

   

    public override string ToIso()
    {
        return $"{_year.ToString()}-{Symbol}{_currentWeek.ToString()}-{_dayWeek.ToString()}";
    }
    

    public override Time Next(int times = 1)
    {
        SetCurrentDyOfWeek(_dayWeek.Next(times).Current().ToString());
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        SetCurrentDyOfWeek(_dayWeek.Prev(times).Current().ToString());
        
        return Clone();
    }

    private void SetCurrentDyOfWeek(string day)
    {
        _dayWeek = new DayWeekSequencer(int.Parse(day));

        _dayWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _currentWeek.Next();

            SetCurrentWeek(sequencer.Current());
        });

        _dayWeek.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _currentWeek.Prev();

            SetCurrentWeek(sequencer.Current());
        });
    }

    private void SetCurrentWeek(int current)
    {
        _currentWeek = new WeekYearSequencer(current);

        _currentWeek.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _year.Next();
            _year = new YearSequencer(sequencer.Current());
        });
    }


    public override string ToString()
    {
        return $"{_year.ToString()}-{_day.ToString()}";
    }

    protected override Time Clone()
    {
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _year.Current();
        yield return _currentWeek.Current();
        yield return _dayWeek.Current();
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
        // ISO week to date: get the first day of the year, add (week-1)*7 days, then add (day-1)
        var jan1 = new DateTime(_year.Current(), 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return jan1.AddDays((_currentWeek.Current() - 1) * 7 + (_dayWeek.Current() - 1));
    }
    protected override void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone)
    {
        _year = new YearSequencer(dateTime.Year);
        var week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dateTime, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
        _currentWeek = new WeekYearSequencer(week);
        _dayWeek = new DayWeekSequencer((int)dateTime.DayOfWeek + 1); // +1 to match 1-based
        this.TimeZoneInfo = zone;
    }
}