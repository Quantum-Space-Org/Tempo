using System;
using System.Collections.Generic;
using System.Linq;
using Quantum.Tempo;
using Quantum.Tempo.Times.Comparers;

namespace TimeSequencer.Times;

public class TimeIntervalFactory
{
    public static Time Create(Time? startsAt, Time? finishesAt)
    {
        switch (startsAt)
        {
            case null when finishesAt is null:
                throw new Exception();
            case null:
                return finishesAt;
            default:
                startsAt.BoundedTo(finishesAt);
                return startsAt;
        }
    }
}

public abstract class Time : Interval
{
    public TimeZoneInfo TimeZoneInfo { get; protected set; }

    public abstract Time Next(int times = 1);


    public virtual Time Later(int times = 1) => Next(times);

    public abstract Time Prev(int times = 1);
    public virtual Time Before(int times = 1) => Prev(times);

    public abstract override string ToString();
    public abstract string ToIso();
    protected abstract Time Clone();

    protected YearSequencer _year;
    protected DaySequencer _day;
    protected MonthSequencer _month;
    protected HourSequencer _hour;
    protected MinuteSequencer _minute;
    protected SecondSequencer _second;

    protected Sequencer _offset;
    protected string _timezone;

    private bool leapYear;
    protected string _offsetValue;

    protected Time(YearSequencer yearSequencer, TimeZoneInfo timeZoneInfo = null) : base(null, null)
    {
        _year = yearSequencer;
        leapYear = DateTime.IsLeapYear(_year.Current());
        TimeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Utc;
        BoundedTo(this);
    }

    protected Time Month(string month)
    {
        _month = ToMonth(int.Parse(month));

        _month.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _year.Next();

            leapYear = DateTime.IsLeapYear(sequencer.Current());

            _year = new YearSequencer(sequencer.Current());
        });

        _month.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _year.Prev();

            leapYear = DateTime.IsLeapYear(sequencer.Current());

            _year = new YearSequencer(sequencer.Current());
        });

        return this;
    }

    protected void DayNextTimes(int times)
        => Day(_day.Next(times).Current().ToString());

    protected void DayPrevTimes(int times)
    {
        if (_day.Current() - times >= _day.Min())
            Day(_day.Prev(times).Current().ToString());
        _day.Prev(times);
    }


    protected void SecondNextTimes(int times)
        => Second(_second.Next(times).Current().ToString());

    protected void SecondPrevTimes(int times)
        => Second(_second.Prev(times).Current().ToString());

    protected void MinuteNextTimes(int times)
        => Minute(_minute.Next(times).Current().ToString());

    protected void MinutePrevTimes(int times)
        => Minute(_minute.Prev(times).Current().ToString());

    protected void HourNextTimes(int times)
        => Hour(_hour.Next(times).Current().ToString());

    protected void HourPrevTimes(int times)
        => Hour(_hour.Prev(times).Current().ToString());

    protected void MonthNextTimes(int times)
        => Month(_month.Next(times).Current().ToString());

    protected void MonthPrevTimes(int times)
        => Month(_month.Prev(times).Current().ToString());

    protected void YearNextTimes(int times)
        => Year(_year.Next(times).Current().ToString());


    protected void YearPrevTimes(int times)
        => Year(_year.Prev(times).Current().ToString());

    private void Year(string value)
    {
        _year = YearSequencer.New(int.Parse(value));
        leapYear = DateTime.IsLeapYear(_year.Current());
    }
    protected Time Day(string day)
    {
        _day = _month.CreateDaySequencer(int.Parse(day));

        _day.SetOnReachAtMaxPosition(() =>
        {
            Month(_month.Next().Current().ToString());

            _day = _month.CreateDaySequencer();
        });

        _day.SetOnReachAtMinPosition(() =>
        {
            Month(_month.Prev().Current().ToString());

            _day = _month.CreateDaySequencer();
        });


        return this;
    }

    protected Time Hour(string hour)
    {
        _hour = new HourSequencer(int.Parse(hour));

        _hour.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _day.Next();
            _day = _month.CreateDaySequencer(sequencer.Current());
        });

        _hour.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _day.Prev();
            _day = _month.CreateDaySequencer(sequencer.Current());
        });

        return this;
    }

    protected Time Minute(string minute)
    {
        _minute = new MinuteSequencer(int.Parse(minute));

        _minute.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _hour.Next();
            _hour = new HourSequencer(sequencer.Current());
        });

        _minute.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _hour.Prev();
            _hour = new HourSequencer(sequencer.Current());
        });

        return this;
    }


    public virtual string NextIso(int times)
    {
        Next(times);
        return ToIso();
    }
    public virtual string PrevIso(int times)
    {
        Prev(times);
        return ToIso();
    }

    protected Time Second(string second)
    {
        _second = new SecondSequencer(int.Parse(second));

        _second.SetOnReachAtMaxPosition(() =>
        {
            var sequencer = _minute.Next();
            _minute = new MinuteSequencer(sequencer.Current());
        });

        _second.SetOnReachAtMinPosition(() =>
        {
            var sequencer = _minute.Prev();
            _minute = new MinuteSequencer(sequencer.Current());
        });

        return this;
    }

    public Time Offset(string offset)
    {
        //_offset = offset;
        _offsetValue = offset;
        //_offset = new Sequencer(1, 2);
        return this;
    }

    public Time TimeZone(string timezone)
    {
        _timezone = timezone;
        return this;
    }

    protected bool IsLeapYear() => leapYear;


    private MonthSequencer ToMonth(int month)
        => month switch
        {
            1 => MonthSequencer.January(),
            2 => MonthSequencer.February(leapYear),
            3 => MonthSequencer.March(),

            4 => MonthSequencer.April(),
            5 => MonthSequencer.May(),
            6 => MonthSequencer.June(),

            7 => MonthSequencer.July(),
            8 => MonthSequencer.August(),
            9 => MonthSequencer.September(),

            10 => MonthSequencer.October(),
            11 => MonthSequencer.November(),
            12 => MonthSequencer.December()
        };


    public int GetYear() => _year.Current();
    public int? GetMonth() => _month?.Current();
    public int? GetDay() => _day?.Current();
    public int? GetHour() => _hour?.Current();
    public int? GetMinute() => _minute?.Current();
    public int? GetSecond() => _second?.Current();


    protected abstract IEnumerable<object> GetEqualityComponents();


    public virtual Time EndOfMonth() => EnclosingMonth().NestDay().Last();
    public virtual Time BeginningOfMonth() => EnclosingMonth().NestDay().First();
    protected abstract TimeSequence NestMonth();
    protected abstract TimeSequence NestDay();

    public TimeSequence Nest(DayMonth dayMonth)
    {
        return dayMonth switch
        {
            DayMonth.Day => NestDay(),
            DayMonth.Month => NestMonth(),
            _ => throw new ArgumentOutOfRangeException(nameof(dayMonth), dayMonth, null)
        };
    }

    public abstract Time EnclosingImmediate();

    public virtual Time EnclosingYear()
        => YearTime.New(GetYear().ToString());

    public virtual Time EnclosingMonth()
        => YearMonthTime.New(GetYear().ToString(), GetMonth().ToString());

    public virtual IntervalRelation CompareTo(Time value)
    {
        if (this.GetType().FullName != value.GetType().FullName)
            throw new InvalidOperationException("Time sequences are not same");

        var dateComparisonResult = DateComparer.New().Compare(this, value);
        switch (dateComparisonResult)
        {
            case < 0:
                return IntervalRelation.Before;
            case > 0:
                return IntervalRelation.After;
        }

        var timeComparisonResult = TimeComparer.New().Compare(this, value);
        return timeComparisonResult switch
        {
            < 0 => IntervalRelation.Before,
            > 0 => IntervalRelation.After,
            _ => IntervalRelation.Equal
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj.GetType() != this.GetType())
            return false;

        var time = ((Time)obj);
        var components = time.GetEqualityComponents();
        var equalityComponents = this.GetEqualityComponents();

        return equalityComponents.SequenceEqual(components);
    }


    public void BoundedTo(Time finishesAt)
    {
        Start= this;
        Finish = finishesAt;
    }

    public Time WithTimeZone(TimeZoneInfo newZone)
    {
        // Convert the current time to DateTime in the current time zone
        var localDateTime = ToDateTime();
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo);
        var newLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, newZone);
        // Create a new instance in the new time zone
        var clone = (Time)Clone();
        clone.SetFromDateTime(newLocalDateTime, newZone);
        return clone;
    }

    protected abstract void SetFromDateTime(DateTime dateTime, TimeZoneInfo zone);
    public abstract DateTime ToDateTime();
}

public enum DayMonth
{
    Day,
    Month
}