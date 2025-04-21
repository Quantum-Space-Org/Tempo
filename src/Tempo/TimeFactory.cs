using TimeSequencer.Times;

namespace Quantum.Tempo;

public class TimeFactory
{
    public static Time Create(string value)
    {
        return value.IsoPattern() switch
        {
            IsoFormatter.Year => Year(value),
            IsoFormatter.YearWeek => YearWeekTime(value),
            IsoFormatter.YearDay => YearDayTime(value),
            IsoFormatter.YearMonthDay => YearMonthDay(value),
            IsoFormatter.YearWeekDay => YearWeekDay(value),
            IsoFormatter.YearMonthDayTimeHourMinute => YearMonthDayTimeHourMinute(value),
            IsoFormatter.YearMonthDayTimeHourMinuteSecond => YearMonthDayTimeHourMinuteSecond(value),
            IsoFormatter.YearMonthDayTimeHour => YearMonthDayTimeHour(value),
            IsoFormatter.YearMonth => YearMonth(value)
        };
    }

    private static Time Year(string value)
    {
        var strings = value.Split("T");

        return (strings.Length == 1 
            ? YearTime.New(value)
            : YearTime.New(strings[0]).Offset(strings[1]));
    }

    private static Time YearMonth(string value)
    {
        /// 2017-02
        return YearMonthTime.New(value.Split("-")[0], value.Split("-")[1]);
    }

    private static Time YearMonthDayTimeHour(string value)
    {
        /// 2017-02-12T20
        var strings = value.Split("T");
        var date = strings[0].Split("-");
        var time = strings[1];

        return YearMonthDayTimeHourTime.New(date[0],
            date[1],
            date[2]
            , time.Split(":")[0]);
    }

    private static Time YearMonthDayTimeHourMinute(string value)
    {
        var valueTuple = value.SplitTime();

        var strings = valueTuple.time.Split("T");
        var date = strings[0].Split("-");
        var time = strings[1];

        var yearMonthDayTimeHourMinuteTime = YearMonthDayTimeHourMinuteTime.New(
            date[0],
            date[1],
            date[2]
            , time.Split(":")[0]
            , time.Split(":")[1]);

        if (valueTuple.offset is null)
        {
            return yearMonthDayTimeHourMinuteTime;
        }

        return yearMonthDayTimeHourMinuteTime.Offset(valueTuple.offset).TimeZone(valueTuple.zone);
    }

    private static Time YearMonthDayTimeHourMinuteSecond(string value)
    {
        var valueTuple = value.SplitTime();

        var strings = valueTuple.time.Split("T");
        var date = strings[0].Split("-");
        var time = strings[1];

        var yearMonthDayTimeHourMinuteTime = YearMonthDayTimeHourMinuteSecondTime.New(
            date[0],
            date[1],
            date[2]
            , time.Split(":")[0]
            , time.Split(":")[1]
            , time.Split(":")[2]);

        if (valueTuple.offset is null)
        {
            return yearMonthDayTimeHourMinuteTime;
        }

        return yearMonthDayTimeHourMinuteTime.Offset(valueTuple.offset).TimeZone(valueTuple.zone);
    }


    private static Time YearWeekDay(string value)
    {
        var strings = value.Split("-");

        return YearWeekDayTime.New(strings[0], strings[1], strings[2]);
    }

    private static Time YearMonthDay(string value)
    {
        var strings = value.Split("-");
        return YearMonthDayTime.New(strings[0], strings[1], strings[2]);
    }

    private static Time YearWeekTime(string value)
    {
        var strings = value.Split("-");

        return Tempo.YearWeekTime.New(strings[0], strings[1]);
    }

    private static Time YearDayTime(string value)
    {
        var strings = value.Split("-");

        return Tempo.YearDayTime.New(strings[0], strings[1]);
    }
}