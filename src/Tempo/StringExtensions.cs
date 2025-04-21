using System;
using System.Text.RegularExpressions;

namespace Quantum.Tempo;

public static class StringExtensions
{
    public static bool IsAValidIso8601(this string value)
    {
        return IsoPattern(value) is not "";
    }

    public static (string time, string offset, string zone) SplitTime(this string value)
    {
        var splitedByTime = value.Split("T");
        
        if (TheValueDoesNotHasTimeValue())
            return (datePart: DatePart(), "", "");

        if (TheValueHasNotOffset())
            return (value, "", "");

        var splittedTimePart = TimePart().Split(OffsetSign());

        var time = splittedTimePart[0];
        var offsetAndZonePart = splittedTimePart[1];

        if (OffsetAndTimeZoneDosNotContainsTimeZone(offsetAndZonePart))
        {
            var timePart = time is "" ? "" : "T" + time;

            if (OffsetAndTimeZoneContainsOffset())
                return (DatePart() + timePart, OffsetSign() + offsetAndZonePart, "");

            return (DatePart() + timePart, "", "");
        }
        
        return (DatePart() + "T" + time, SplitOffsetTimeZone(offsetAndZonePart).offset, SplitOffsetTimeZone(offsetAndZonePart).timeZone);




        (string offset, string timeZone) SplitOffsetTimeZone(string value)
        {
            var offset = OffsetSign() + value.Split("[")[0];
            var timeZone = "[" + value.Split("[")[1];
            return (offset, timeZone);
        }

        string DatePart()
        {
            return splitedByTime[0];
        }

        string TimePart()
        {
            var timePart = "";
            if (splitedByTime.Length > 1)
                timePart = splitedByTime[1];
            return timePart;
        }

        bool TheValueDoesNotHasTimeValue()
        {
            return TimePart() is "";
        }

        bool TheValueHasNotOffset()
        {
            return TimePart().Contains("-") is false && TimePart().Contains("+") is false;
        }

        string OffsetSign()
        {
            var offsetSign = "";
            if (TimePart().Contains("-"))
                offsetSign = "-";
            else if (TimePart().Contains("+"))
                offsetSign = "+";
            return offsetSign;
        }

        bool OffsetAndTimeZoneDosNotContainsTimeZone(string value)
        {
            return value.Contains("[") is false;
        }

        bool OffsetAndTimeZoneContainsOffset()
        {
            return offsetAndZonePart != "";
        }
    }

    public static string IsoPattern(this string value)
    {

        var time = SplitTime(value).time;

        if (new Regex(@"\d\d\d\d-W\d\d-\d").IsMatch(time))
            return IsoFormatter.YearWeekDay;

        if (new Regex(@"\d\d\d\d-W\d\d").IsMatch(time))
            return IsoFormatter.YearWeek;

        if (new Regex(@"\d\d\d\d-\d\d\d").IsMatch(time))
            return IsoFormatter.YearDay;

        if (new Regex(@"\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d").IsMatch(time))
            return IsoFormatter.YearMonthDayTimeHourMinuteSecond;

        if (new Regex(@"\d\d\d\d-\d\d-\d\dT\d\d:\d\d").IsMatch(time))
            return IsoFormatter.YearMonthDayTimeHourMinute;
        
        if (new Regex(@"\d\d\d\d-\d\d-\d\dT\d\d").IsMatch(time))
            return IsoFormatter.YearMonthDayTimeHour;

        if (new Regex(@"\d\d\d\d-\d\d-\d\d").IsMatch(time))
            return IsoFormatter.YearMonthDay;

        if (new Regex(@"\d\d\d\d-\d\d").IsMatch(time))
            return IsoFormatter.YearMonth;

        if (new Regex(@"\d\d\d\d").IsMatch(time))
            return IsoFormatter.Year;


        throw new Exception();
    }
}