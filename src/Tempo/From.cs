using System;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class From
{
    private readonly string _value;

    private From(string value) => _value = value;

    public static From Iso(string s) => new(s);

    public string Next(int times = 1) 
        => _value.FromIso().ToTime().NextIso(times);
  public string Later(int times = 1) 
        => _value.FromIso().ToTime().Later(times).ToIso();

  public string Before(int times = 1)
      => _value.FromIso().ToTime().Before(times).ToIso();

    public string Prev(int times = 1) 
        => _value.FromIso().ToTime().PrevIso(times);

    public DateTime ToDateTime() 
        => ToDateTime(_value);


    public Time ToTime() 
        => TimeFactory.Create(_value);

    public Time ToTimeString()
    {
        var timeInterval = ToTimeInterval();

        return TimeIntervalFactory.Create(timeInterval.Start, timeInterval.Finish);

    }

    private static Time ToTime(string value) 
        => TimeFactory.Create(value);

 
    private static DateTime ToDateTime(string value) 
        => IsoDateTimeFactory.Create(value);

    public Interval ToTimeInterval()
    {
        var interval= ComposedToInterval(_value);
        Guard(interval);


        return new Interval(interval.starts, interval.finiehses);

        var value = _value.Trim();
        if (value.StartsWith("-"))
        {
            var substring = value.Substring(value.IndexOf("/") + 1);
            return new Interval(null, ToTime(substring));
        }


        if (value.EndsWith("-"))
            return new Interval(ToTime(value.Split("/")[0]), null);

        if (value.LastIndexOf("/") < value.IndexOf("]"))
            return new Interval(ToTime(value), null);

        var strings = _value.Split("/");
        if (strings.Length == 1)
            return new Interval(ToTime(strings[0]), ToTime(strings[0]));
        if (strings.Length == 2)
            return new Interval(strings[0] == "-" ? null : ToTime(strings[0]), strings[1] == "-" ? null : ToTime(strings[1]));

        else
        {
            if (value.IndexOf("[") > value.IndexOf("/"))
            {
                var s = strings[0];
                var indexOf = value.IndexOf("/");
                var substring = value.Substring(indexOf + 1);
                return new Interval(ToTime(s), ToTime(substring));
            }
            else
            {
                var indexOf = value.IndexOf("]");
                var substring = value.Substring(0, indexOf + 1);

                var startIndex = value.IndexOf("]");
                var substring1 = value[(startIndex + 1)..];

                var s = substring1[(substring1.IndexOf("/") + 1)..];

                return new Interval(ToTime(substring), ToTime(s));
            }
        }
    }
    
    public string[] Sequences()
    {
        var timeInterval = ToTimeInterval();

        return  TimeIntervalFactory.Create(timeInterval.Start , timeInterval.Finish).Sequence();
        
    }
    
    public string[] ReverseSequences()
    {
        var timeInterval = ToTimeInterval();

        return TimeIntervalFactory.Create(timeInterval.Start, timeInterval.Finish).ReverseSequence();
    }

    public bool Guard()
    {
        var interval = ComposedToInterval(_value);

        return Guard(interval);
    }

    private bool Guard((Time? starts, Time? finiehses) interval) 
        => interval.starts is not null || interval.finiehses != null;

    (Time? starts, Time? finiehses) ComposedToInterval(string _value)
    {
        var value = _value.Trim();
        if (value.StartsWith("-"))
        {
            var substring = value.Substring(value.IndexOf("/") + 1);
            return (null, ToTime(substring));
        }


        if (value.EndsWith("-"))
            return (ToTime(value.Split("/")[0]), null);

        if (value.LastIndexOf("/") < value.IndexOf("]"))
            return (ToTime(value), null);

        var strings = _value.Split("/");
        if (strings.Length == 1)
            return (ToTime(strings[0]), ToTime(strings[0]));
        if (strings.Length == 2)
            return (strings[0] == "-" ? null : ToTime(strings[0]),
                strings[1] == "-" ? null : ToTime(strings[1]));

        else
        {
            if (value.IndexOf("[") > value.IndexOf("/"))
            {
                var s = strings[0];
                var indexOf = value.IndexOf("/");
                var substring = value.Substring(indexOf + 1);
                return (ToTime(s), ToTime(substring));
            }
            else
            {
                var indexOf = value.IndexOf("]");
                var substring = value.Substring(0, indexOf + 1);

                var startIndex = value.IndexOf("]");
                var substring1 = value[(startIndex + 1)..];

                var s = substring1[(substring1.IndexOf("/") + 1)..];

                return (ToTime(substring), ToTime(s));
            }
        }
    }
}