using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class YearMonthTime : Time
{

    public static YearMonthTime New(string year, string month) => new(year, month);

    public YearMonthTime(string year, string month) : base(YearSequencer.New(int.Parse(year)))
    {
        base.Month(month);
    }

    public override string ToIso()
    {
        return $"{_year.ToString()}-{_month.ToString()}";
    }

    public override Time Next(int times = 1)
    {
        MonthNextTimes(times);
        return Clone();
    }

    public override Time Prev(int times = 1)
    {
        MonthPrevTimes(times);
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


    protected override TimeSequence NestMonth() => new(this, 1);

    protected override TimeSequence NestDay()
    {
        var daySequencer = this._month.CreateDaySequencer();
        return new TimeSequence(new YearMonthDayTime(this._year.Current().ToString(), this._month.Current().ToString(), "1"), daySequencer.Max());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new System.NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        return this._year.Current() == ((YearMonthTime)obj)._year.Current()
               && this._month.Current() == ((YearMonthTime)obj)._month.Current();
    }
    
    public override Time EnclosingImmediate()
    {
        return YearTime.New(this.GetYear().ToString());
    }
}
