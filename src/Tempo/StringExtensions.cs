using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic; // Added for List
using System.Linq;

namespace Quantum.Tempo;

public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is a valid ISO 8601 date/time string.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is a valid ISO 8601 date/time string, otherwise false.</returns>
    public static bool IsAValidIso8601(this string value)
    {
        return IsoPattern(value) is not "";
    }

    /// <summary>
    /// Splits a date/time string into its date and time parts.
    /// </summary>
    /// <param name="value">The date/time string to split (e.g., '2024-05-01T14:30+03:30').</param>
    /// <returns>A tuple containing the date part, offset part, and time zone part.</returns>
    public static (string datePart, string offset, string zone) SplitTime(this string value)
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

    /// <summary>
    /// Determines the ISO 8601 pattern for a given date/time string.
    /// </summary>
    /// <param name="value">The date/time string to analyze.</param>
    /// <returns>A string representing the ISO 8601 pattern.</returns>
    public static string IsoPattern(this string value)
    {

        var time = SplitTime(value).datePart;

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

    /// <summary>
    /// Normalizes a date string to a standard ISO 8601 format (yyyy-MM-dd).
    /// </summary>
    /// <param name="value">The date string to normalize (e.g., '2017.2.27', '2017/2/27', '13/2/27', '2017-2-27').</param>
    /// <returns>A normalized ISO 8601 date string.</returns>
    public static string NormalizeToIsoDateString(this string value)
    {
        // Try to parse directly
        if (DateTime.TryParse(value, out var dt))
            return dt.ToString("yyyy-MM-dd");

        // Try to parse common non-ISO formats (e.g., 2017.2.27, 2017/2/27, 13/2/27, 2017-2-27, etc.)
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3)
        {
            var year = parts[0].Length == 2 ? $"20{parts[0]}" : parts[0];
            int p1 = int.Parse(parts[1]);
            int p2 = int.Parse(parts[2]);
            string month, day;
            // If the second part is > 12, it's likely the day, not the month
            if (p1 > 12)
            {
                day = p1.ToString("D2");
                month = p2.ToString("D2");
            }
            else if (p2 > 12)
            {
                month = p1.ToString("D2");
                day = p2.ToString("D2");
            }
            else
            {
                month = p1.ToString("D2");
                day = p2.ToString("D2");
            }
            return $"{year}-{month}-{day}";
        }
        throw new FormatException($"Cannot normalize date: {value}");
    }

    /// <summary>
    /// Normalizes a Persian date string (e.g., 1402/02/01) to a standard ISO 8601 format (yyyy-MM-dd).
    /// </summary>
    /// <param name="value">The Persian date string to normalize.</param>
    /// <returns>A normalized ISO 8601 date string.</returns>
    public static string NormalizeToPersianIsoDateString(this string value)
    {
        // Accepts Persian date strings like 1402/02/01 or 1402-2-1
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3)
        {
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);
            // Try to convert to Gregorian using .NET PersianCalendar
            try
            {
                var pc = new PersianCalendar();
                var gdt = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                return gdt.ToString("yyyy-MM-dd");
            }
            catch
            {
                // Fallback: just pad and return as Persian ISO
                return $"{year:D4}-{month:D2}-{day:D2}";
            }
        }
        throw new FormatException($"Cannot normalize Persian date: {value}");
    }

    /// <summary>
    /// Normalizes a Hijri date string (e.g., 1445/10/01) to a standard ISO 8601 format (yyyy-MM-dd).
    /// </summary>
    /// <param name="value">The Hijri date string to normalize.</param>
    /// <returns>A normalized ISO 8601 date string.</returns>
    public static string NormalizeToHijriIsoDateString(this string value)
    {
        // Accepts Hijri date strings like 1445/10/01 or 1445-10-1
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3)
        {
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);
            // Try to convert to Gregorian using .NET HijriCalendar
            try
            {
                var hc = new HijriCalendar();
                var gdt = hc.ToDateTime(year, month, day, 0, 0, 0, 0);
                return gdt.ToString("yyyy-MM-dd");
            }
            catch
            {
                // Fallback: just pad and return as Hijri ISO
                return $"{year:D4}-{month:D2}-{day:D2}";
            }
        }
        throw new FormatException($"Cannot normalize Hijri date: {value}");
    }

    /// <summary>
    /// Auto-detects the calendar system (Gregorian, Persian, Hijri) from the input string and normalizes to ISO Gregorian yyyy-MM-dd string.
    /// Heuristics:
    ///   - Year 1300-1499: Persian (Jalali)
    ///   - Year 1400-1600: Hijri (Islamic)
    ///   - Year 1800-2100: Gregorian
    ///   - Otherwise, tries Gregorian, then Persian, then Hijri.
    /// Use explicit NormalizeToPersianIsoDateString or NormalizeToHijriIsoDateString for critical cases.
    /// </summary>
    /// <param name="value">The input date string to normalize.</param>
    /// <returns>A normalized ISO Gregorian date string.</returns>
    public static string AutoNormalizeToIsoDateString(this string value)
    {
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[0], out int year))
        {
            if (year >= 1300 && year <= 1499)
                return value.NormalizeToPersianIsoDateString();
            if (year >= 1400 && year <= 1600)
                return value.NormalizeToHijriIsoDateString();
            if (year >= 1800 && year <= 2100)
                return value.NormalizeToIsoDateString();
            // Fallback: try Gregorian, then Persian, then Hijri
            try { return value.NormalizeToIsoDateString(); } catch { }
            try { return value.NormalizeToPersianIsoDateString(); } catch { }
            try { return value.NormalizeToHijriIsoDateString(); } catch { }
        }
        // If not a recognized format, fallback to Gregorian normalization
        return value.NormalizeToIsoDateString();
    }

    /// <summary>
    /// Parses fuzzy/relative date expressions (e.g., 'today', 'tomorrow', 'yesterday', 'next Friday', 'last Monday') to a normalized date string.
    /// Optionally takes a reference date (defaults to today).
    /// </summary>
    /// <param name="input">The relative date expression to parse (e.g., 'today', 'tomorrow', 'yesterday').</param>
    /// <param name="referenceDate">An optional reference date string (e.g., '2024-05-01') to use as a base.</param>
    /// <returns>A normalized date string.</returns>
    public static string ParseRelativeDate(string input, string? referenceDate = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty.");
        var refDate = referenceDate == null
            ? DateTime.Today
            : DateTime.Parse(referenceDate.AutoNormalizeToIsoDateString());
        var lower = input.Trim().ToLowerInvariant();
        if (lower == "today")
            return refDate.ToString("yyyy-MM-dd");
        if (lower == "tomorrow")
            return refDate.AddDays(1).ToString("yyyy-MM-dd");
        if (lower == "yesterday")
            return refDate.AddDays(-1).ToString("yyyy-MM-dd");
        // next/last/this <dayofweek>
        var daysOfWeek = Enum.GetNames(typeof(DayOfWeek));
        foreach (var prefix in new[] { "next ", "last ", "this " })
        {
            if (lower.StartsWith(prefix))
            {
                var dayName = lower.Substring(prefix.Length).Trim();
                var match = Array.Find(daysOfWeek, d => d.ToLowerInvariant() == dayName);
                if (match != null)
                {
                    var targetDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), match);
                    int diff = targetDay - refDate.DayOfWeek;
                    if (prefix == "next ")
                        diff = (diff <= 0 ? 7 : 0) + diff;
                    else if (prefix == "last ")
                        diff = (diff >= 0 ? -7 : 0) + diff;
                    // 'this' means this week's occurrence (may be today or past)
                    return refDate.AddDays(diff).ToString("yyyy-MM-dd");
                }
            }
        }
        throw new FormatException($"Unrecognized relative date expression: {input}");
    }

    /// <summary>
    /// Converts a Gregorian date string (yyyy-MM-dd) to a Persian (Jalali) date string (yyyy-MM-dd).
    /// </summary>
    /// <param name="gregorianIso">The Gregorian date string to convert (e.g., '2024-05-01').</param>
    /// <returns>A Persian date string.</returns>
    public static string ToPersianString(this string gregorianIso)
    {
        if (!DateTime.TryParse(gregorianIso, out var dt))
            throw new FormatException($"Cannot parse Gregorian date: {gregorianIso}");
        var pc = new PersianCalendar();
        var year = pc.GetYear(dt);
        var month = pc.GetMonth(dt);
        var day = pc.GetDayOfMonth(dt);
        return $"{year:D4}-{month:D2}-{day:D2}";
    }

    /// <summary>
    /// Converts a Gregorian date string (yyyy-MM-dd) to a Hijri (Islamic) date string (yyyy-MM-dd).
    /// Note: .NET HijriCalendar may differ by a day from civil/astronomical calendars.
    /// </summary>
    /// <param name="gregorianIso">The Gregorian date string to convert (e.g., '2024-05-01').</param>
    /// <returns>A Hijri date string.</returns>
    public static string ToHijriString(this string gregorianIso)
    {
        if (!DateTime.TryParse(gregorianIso, out var dt))
            throw new FormatException($"Cannot parse Gregorian date: {gregorianIso}");
        var hc = new HijriCalendar();
        var year = hc.GetYear(dt);
        var month = hc.GetMonth(dt);
        var day = hc.GetDayOfMonth(dt);
        return $"{year:D4}-{month:D2}-{day:D2}";
    }

    /// <summary>
    /// Returns the next date string after the given date, using auto calendar detection.
    /// </summary>
    /// <param name="date">The date string to increment (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to increment (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string NextDate(this string date, int times = 1)
    {
        // Normalize to ISO Gregorian, add days, then format back to original calendar
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var next = dt.AddDays(times);
        // If original was Persian
        if (date.IsPersianDateString())
            return next.ToString("yyyy-MM-dd").ToPersianString();
        // If original was Hijri
        if (date.IsHijriDateString())
            return next.ToString("yyyy-MM-dd").ToHijriString();
        // Default: Gregorian
        return next.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the previous date string before the given date, using auto calendar detection.
    /// </summary>
    /// <param name="date">The date string to decrement (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to decrement (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string PrevDate(this string date, int times = 1)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var prev = dt.AddDays(-times);
        if (date.IsPersianDateString())
            return prev.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return prev.ToString("yyyy-MM-dd").ToHijriString();
        return prev.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns a list of date strings from start to end (inclusive), using auto calendar detection.
    /// </summary>
    /// <param name="start">The start date string (e.g., '2024-05-01').</param>
    /// <param name="end">The end date string (e.g., '2024-05-05').</param>
    /// <returns>A list of date strings in the same calendar as the input.</returns>
    public static List<string> Sequence(string start, string end)
    {
        var isoStart = start.AutoNormalizeToIsoDateString();
        var isoEnd = end.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoStart, out var dtStart) || !DateTime.TryParse(isoEnd, out var dtEnd))
            throw new FormatException($"Cannot parse start or end date: {start}, {end}");
        var result = new List<string>();
        var current = dtStart;
        while (current <= dtEnd)
        {
            if (start.IsPersianDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToPersianString());
            else if (start.IsHijriDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToHijriString());
            else
                result.Add(current.ToString("yyyy-MM-dd"));
            current = current.AddDays(1);
        }
        return result;
    }

    /// <summary>
    /// Heuristic: Returns true if the string is likely a Persian date.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is likely a Persian date, otherwise false.</returns>
    public static bool IsPersianDateString(this string value)
    {
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[0], out int year))
            return year >= 1300 && year <= 1499;
        return false;
    }

    /// <summary>
    /// Heuristic: Returns true if the string is likely a Hijri date.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is likely a Hijri date, otherwise false.</returns>
    public static bool IsHijriDateString(this string value)
    {
        var cleaned = value.Replace("/", "-").Replace(".", "-");
        var parts = cleaned.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[0], out int year))
            return year >= 1400 && year <= 1600;
        return false;
    }

    /// <summary>
    /// Adds a duration string to a date/time string, returning the resulting date/time string (same calendar as input).
    /// </summary>
    /// <param name="dateTimeString">The date/time string to add duration to (e.g., '2024-05-01T14:30+03:30').</param>
    /// <param name="durationString">The duration string to add (e.g., 'PT1H30M').</param>
    /// <returns>A date/time string in the same calendar as the input.</returns>
    public static string AddDuration(this string dateTimeString, string durationString)
    {
        var iso = dateTimeString.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date/time: {dateTimeString}");
        var duration = Quantum.Tempo.Times.Duration.Parse(durationString);
        var result = dt.AddSeconds(duration.ToTotalSeconds());
        if (dateTimeString.IsPersianDateString())
            return result.ToString("yyyy-MM-dd").ToPersianString();
        if (dateTimeString.IsHijriDateString())
            return result.ToString("yyyy-MM-dd").ToHijriString();
        return result.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Subtracts a duration string from a date/time string, returning the resulting date/time string (same calendar as input).
    /// </summary>
    /// <param name="dateTimeString">The date/time string to subtract duration from (e.g., '2024-05-01T14:30+03:30').</param>
    /// <param name="durationString">The duration string to subtract (e.g., 'PT1H30M').</param>
    /// <returns>A date/time string in the same calendar as the input.</returns>
    public static string SubtractDuration(this string dateTimeString, string durationString)
    {
        var iso = dateTimeString.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date/time: {dateTimeString}");
        var duration = Quantum.Tempo.Times.Duration.Parse(durationString);
        var result = dt.AddSeconds(-duration.ToTotalSeconds());
        if (dateTimeString.IsPersianDateString())
            return result.ToString("yyyy-MM-dd").ToPersianString();
        if (dateTimeString.IsHijriDateString())
            return result.ToString("yyyy-MM-dd").ToHijriString();
        return result.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Generates a sequence of date strings from start to end (inclusive), stepping by a duration string (same calendar as input).
    /// </summary>
    /// <param name="startDate">The start date string (e.g., '2024-05-01').</param>
    /// <param name="endDate">The end date string (e.g., '2024-05-05').</param>
    /// <param name="durationString">The duration string to step by (e.g., 'PT1H').</param>
    /// <returns>A list of date strings in the same calendar as the input.</returns>
    public static List<string> SequenceByDuration(this string startDate, string endDate, string durationString)
    {
        var isoStart = startDate.AutoNormalizeToIsoDateString();
        var isoEnd = endDate.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoStart, out var dtStart) || !DateTime.TryParse(isoEnd, out var dtEnd))
            throw new FormatException($"Cannot parse start or end date: {startDate}, {endDate}");
        var duration = Quantum.Tempo.Times.Duration.Parse(durationString);
        var result = new List<string>();
        var current = dtStart;
        while (current <= dtEnd)
        {
            if (startDate.IsPersianDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToPersianString());
            else if (startDate.IsHijriDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToHijriString());
            else
                result.Add(current.ToString("yyyy-MM-dd"));
            current = current.AddSeconds(duration.ToTotalSeconds());
        }
        return result;
    }

    /// <summary>
    /// Parses a time-of-day string (e.g., '14:30', '14:30:00') and returns a normalized 'HH:mm[:ss]' string.
    /// </summary>
    /// <param name="timeString">The time string to parse (e.g., '14:30', '14:30:00').</param>
    /// <returns>A normalized time string.</returns>
    public static string ParseTimeOfDay(string timeString)
    {
        if (string.IsNullOrWhiteSpace(timeString))
            throw new ArgumentException("Time string cannot be null or empty.");
        var parts = timeString.Split(':');
        if (parts.Length == 2)
        {
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            return $"{hour:D2}:{minute:D2}";
        }
        if (parts.Length == 3)
        {
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            int second = int.Parse(parts[2]);
            return $"{hour:D2}:{minute:D2}:{second:D2}";
        }
        throw new FormatException($"Invalid time-of-day format: {timeString}");
    }

    /// <summary>
    /// Converts a 'yyyy-MM' string to a human-readable month string in the specified language (e.g., '2024-05' -> 'May 2024').
    /// Supports 'en' (English), 'fa' (Farsi), 'ar' (Arabic).
    /// </summary>
    /// <param name="yearMonth">The year-month string to convert (e.g., '2024-05').</param>
    /// <param name="languageCode">The language code for localization (e.g., 'en', 'fa', 'ar').</param>
    /// <returns>A human-readable month string.</returns>
    public static string ToHumanMonth(this string yearMonth, string languageCode = "en")
    {
        var parts = yearMonth.Split('-');
        if (parts.Length != 2)
            throw new FormatException($"Invalid year-month format: {yearMonth}");
        int year = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        string monthName;
        switch (languageCode.ToLowerInvariant())
        {
            case "fa":
                monthName = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" }[month - 1];
                return $"{monthName} {year}";
            case "ar":
                monthName = new[] { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" }[month - 1];
                return $"{monthName} {year}";
            default:
                monthName = new DateTime(year, month, 1).ToString("MMMM");
                return $"{monthName} {year}";
        }
    }

    /// <summary>
    /// Converts a 'yyyy' string to a human-readable year string (e.g., '2024' -> '2024').
    /// </summary>
    /// <param name="year">The year string to convert (e.g., '2024').</param>
    /// <returns>A human-readable year string.</returns>
    public static string ToHumanYear(this string year)
    {
        if (!int.TryParse(year, out _))
            throw new FormatException($"Invalid year format: {year}");
        return year;
    }

    /// <summary>
    /// Converts a 'yyyy-Www' string to a human-readable week string in the specified language (e.g., '2024-W23' -> 'Week 23 of 2024').
    /// Supports 'en' (English), 'fa' (Farsi), 'ar' (Arabic).
    /// </summary>
    /// <param name="yearWeek">The year-week string to convert (e.g., '2024-W23').</param>
    /// <param name="languageCode">The language code for localization (e.g., 'en', 'fa', 'ar').</param>
    /// <returns>A human-readable week string.</returns>
    public static string ToHumanWeek(this string yearWeek, string languageCode = "en")
    {
        var parts = yearWeek.Split('-');
        if (parts.Length != 2 || !parts[1].StartsWith("W"))
            throw new FormatException($"Invalid year-week format: {yearWeek}");
        int year = int.Parse(parts[0]);
        int week = int.Parse(parts[1].Substring(1));
        switch (languageCode.ToLowerInvariant())
        {
            case "fa":
                return $"هفته {week} سال {year}";
            case "ar":
                return $"الأسبوع {week} من {year}";
            default:
                return $"Week {week} of {year}";
        }
    }

    /// <summary>
    /// Converts a 'yyyy-DDD' string to a human-readable day-of-year string (e.g., '2024-123' -> 'Day 123 of 2024').
    /// </summary>
    /// <param name="yearDay">The year-day string to convert (e.g., '2024-123').</param>
    /// <returns>A human-readable day-of-year string.</returns>
    public static string ToHumanDayOfYear(this string yearDay)
    {
        var parts = yearDay.Split('-');
        if (parts.Length != 2)
            throw new FormatException($"Invalid year-day format: {yearDay}");
        int year = int.Parse(parts[0]);
        int day = int.Parse(parts[1]);
        return $"Day {day} of {year}";
    }

    /// <summary>
    /// Returns the next week string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to increment (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to increment (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string NextWeek(this string dateString, int times = 1)
    {
        var iso = dateString.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {dateString}");
        var result = dt.AddDays(7 * times);
        if (dateString.IsPersianDateString())
            return result.ToString("yyyy-MM-dd").ToPersianString();
        if (dateString.IsHijriDateString())
            return result.ToString("yyyy-MM-dd").ToHijriString();
        return result.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the previous week string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to decrement (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to decrement (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string PrevWeek(this string dateString, int times = 1)
    {
        return dateString.NextWeek(-times);
    }

    /// <summary>
    /// Returns the next month string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to increment (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to increment (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string NextMonth(this string dateString, int times = 1)
    {
        var iso = dateString.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {dateString}");
        var result = dt.AddMonths(times);
        if (dateString.IsPersianDateString())
            return result.ToString("yyyy-MM-dd").ToPersianString();
        if (dateString.IsHijriDateString())
            return result.ToString("yyyy-MM-dd").ToHijriString();
        return result.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the previous month string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to decrement (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to decrement (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string PrevMonth(this string dateString, int times = 1)
    {
        return dateString.NextMonth(-times);
    }

    /// <summary>
    /// Returns the next year string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to increment (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to increment (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string NextYear(this string dateString, int times = 1)
    {
        var iso = dateString.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {dateString}");
        var result = dt.AddYears(times);
        if (dateString.IsPersianDateString())
            return result.ToString("yyyy-MM-dd").ToPersianString();
        if (dateString.IsHijriDateString())
            return result.ToString("yyyy-MM-dd").ToHijriString();
        return result.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the previous year string (same calendar as input).
    /// </summary>
    /// <param name="dateString">The date string to decrement (e.g., '2024-05-01').</param>
    /// <param name="times">The number of times to decrement (default: 1).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string PrevYear(this string dateString, int times = 1)
    {
        return dateString.NextYear(-times);
    }

    /// <summary>
    /// Generates a sequence of month strings from start to end (inclusive), same calendar as input.
    /// </summary>
    /// <param name="start">The start date string (e.g., '2024-05-01').</param>
    /// <param name="end">The end date string (e.g., '2024-05-05').</param>
    /// <returns>A list of date strings in the same calendar as the input.</returns>
    public static List<string> SequenceByMonth(this string start, string end)
    {
        var isoStart = start.AutoNormalizeToIsoDateString();
        var isoEnd = end.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoStart, out var dtStart) || !DateTime.TryParse(isoEnd, out var dtEnd))
            throw new FormatException($"Cannot parse start or end date: {start}, {end}");
        var result = new List<string>();
        var current = new DateTime(dtStart.Year, dtStart.Month, 1);
        var endMonth = new DateTime(dtEnd.Year, dtEnd.Month, 1);
        while (current <= endMonth)
        {
            if (start.IsPersianDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToPersianString());
            else if (start.IsHijriDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToHijriString());
            else
                result.Add(current.ToString("yyyy-MM-dd"));
            current = current.AddMonths(1);
        }
        return result;
    }

    /// <summary>
    /// Generates a sequence of year strings from start to end (inclusive), same calendar as input.
    /// </summary>
    /// <param name="start">The start date string (e.g., '2024-05-01').</param>
    /// <param name="end">The end date string (e.g., '2024-05-05').</param>
    /// <returns>A list of date strings in the same calendar as the input.</returns>
    public static List<string> SequenceByYear(this string start, string end)
    {
        var isoStart = start.AutoNormalizeToIsoDateString();
        var isoEnd = end.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoStart, out var dtStart) || !DateTime.TryParse(isoEnd, out var dtEnd))
            throw new FormatException($"Cannot parse start or end date: {start}, {end}");
        var result = new List<string>();
        var current = new DateTime(dtStart.Year, 1, 1);
        var endYear = new DateTime(dtEnd.Year, 1, 1);
        while (current <= endYear)
        {
            if (start.IsPersianDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToPersianString());
            else if (start.IsHijriDateString())
                result.Add(current.ToString("yyyy-MM-dd").ToHijriString());
            else
                result.Add(current.ToString("yyyy-MM-dd"));
            current = current.AddYears(1);
        }
        return result;
    }

    /// <summary>
    /// Compares two date/time strings. Returns -1 if a < b, 0 if a == b, 1 if a > b (auto calendar detection).
    /// </summary>
    /// <param name="a">The first date/time string to compare.</param>
    /// <param name="b">The second date/time string to compare.</param>
    /// <returns>An integer indicating the comparison result.</returns>
    public static int CompareDates(string a, string b)
    {
        var isoA = a.AutoNormalizeToIsoDateString();
        var isoB = b.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoA, out var dtA) || !DateTime.TryParse(isoB, out var dtB))
            throw new FormatException($"Cannot parse date(s): {a}, {b}");
        return dtA.CompareTo(dtB);
    }

    /// <summary>
    /// Returns true if a is before b (auto calendar detection).
    /// </summary>
    /// <param name="a">The first date/time string.</param>
    /// <param name="b">The second date/time string.</param>
    /// <returns>True if a is before b, otherwise false.</returns>
    public static bool IsBefore(string a, string b) => CompareDates(a, b) < 0;

    /// <summary>
    /// Returns true if a is after b (auto calendar detection).
    /// </summary>
    /// <param name="a">The first date/time string.</param>
    /// <param name="b">The second date/time string.</param>
    /// <returns>True if a is after b, otherwise false.</returns>
    public static bool IsAfter(string a, string b) => CompareDates(a, b) > 0;

    /// <summary>
    /// Returns true if a is equal to b (auto calendar detection).
    /// </summary>
    /// <param name="a">The first date/time string.</param>
    /// <param name="b">The second date/time string.</param>
    /// <returns>True if a is equal to b, otherwise false.</returns>
    public static bool IsEqualDate(string a, string b) => CompareDates(a, b) == 0;

    /// <summary>
    /// Returns true if the date string is within the interval string (inclusive).
    /// </summary>
    /// <param name="date">The date string to check (e.g., '2024-05-01').</param>
    /// <param name="interval">The interval string (e.g., '2024-05-01/2024-05-05').</param>
    /// <returns>True if the date is within the interval, otherwise false.</returns>
    public static bool IsWithinInterval(string date, string interval)
    {
        var parsed = Quantum.Tempo.Interval.Parse(interval);
        var isoDate = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoDate, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var start = parsed.Start?.ToIso();
        var finish = parsed.Finish?.ToIso();
        if (start != null && DateTime.TryParse(start, out var dtStart) && dt < dtStart)
            return false;
        if (finish != null && DateTime.TryParse(finish, out var dtFinish) && dt > dtFinish)
            return false;
        return true;
    }

    /// <summary>
    /// Returns true if the interval string contains the date or another interval string (inclusive).
    /// </summary>
    /// <param name="interval">The outer interval string (e.g., '2024-05-01/2024-05-05').</param>
    /// <param name="dateOrInterval">The date or interval string to check (e.g., '2024-05-03', '2024-05-01/2024-05-05').</param>
    /// <returns>True if the interval contains the date or another interval, otherwise false.</returns>
    public static bool IntervalContains(string interval, string dateOrInterval)
    {
        var outer = Quantum.Tempo.Interval.Parse(interval);
        if (dateOrInterval.Contains("/"))
        {
            var inner = Quantum.Tempo.Interval.Parse(dateOrInterval);
            var outerStart = outer.Start?.ToIso();
            var outerFinish = outer.Finish?.ToIso();
            var innerStart = inner.Start?.ToIso();
            var innerFinish = inner.Finish?.ToIso();
            if (outerStart != null && innerStart != null && DateTime.Parse(innerStart) < DateTime.Parse(outerStart))
                return false;
            if (outerFinish != null && innerFinish != null && DateTime.Parse(innerFinish) > DateTime.Parse(outerFinish))
                return false;
            return true;
        }
        else
        {
            return IsWithinInterval(dateOrInterval, interval);
        }
    }

    /// <summary>
    /// Splits an interval string into N equal sub-intervals, returns list of interval strings.
    /// </summary>
    /// <param name="interval">The interval string to split (e.g., '2024-05-01/2024-05-05').</param>
    /// <param name="parts">The number of parts to split into.</param>
    /// <returns>A list of interval strings.</returns>
    public static List<string> SplitIntervalByParts(string interval, int parts)
    {
        var parsed = Quantum.Tempo.Interval.Parse(interval);
        if (parsed.Start == null || parsed.Finish == null)
            throw new ArgumentException("Interval must have both start and finish to split.");
        var startIso = parsed.Start.ToIso();
        var finishIso = parsed.Finish.ToIso();
        if (!DateTime.TryParse(startIso, out var dtStart) || !DateTime.TryParse(finishIso, out var dtFinish))
            throw new FormatException($"Cannot parse interval endpoints: {interval}");
        var totalSeconds = (dtFinish - dtStart).TotalSeconds;
        if (parts < 1 || totalSeconds < parts)
            throw new ArgumentException("Invalid number of parts or interval too short.");
        var chunkSeconds = totalSeconds / parts;
        var result = new List<string>();
        for (int i = 0; i < parts; i++)
        {
            var chunkStart = dtStart.AddSeconds(i * chunkSeconds);
            var chunkEnd = (i == parts - 1) ? dtFinish : dtStart.AddSeconds((i + 1) * chunkSeconds);
            result.Add($"{chunkStart:yyyy-MM-dd}/{chunkEnd:yyyy-MM-dd}");
        }
        return result;
    }

    /// <summary>
    /// Splits an interval string into sub-intervals of the given duration, returns list of interval strings.
    /// </summary>
    /// <param name="interval">The interval string to split (e.g., '2024-05-01/2024-05-05').</param>
    /// <param name="duration">The duration string to step by (e.g., 'PT1H').</param>
    /// <returns>A list of interval strings.</returns>
    public static List<string> SplitIntervalByDuration(string interval, string duration)
    {
        var parsed = Quantum.Tempo.Interval.Parse(interval);
        if (parsed.Start == null || parsed.Finish == null)
            throw new ArgumentException("Interval must have both start and finish to split.");
        var startIso = parsed.Start.ToIso();
        var finishIso = parsed.Finish.ToIso();
        if (!DateTime.TryParse(startIso, out var dtStart) || !DateTime.TryParse(finishIso, out var dtFinish))
            throw new FormatException($"Cannot parse interval endpoints: {interval}");
        var dur = Quantum.Tempo.Times.Duration.Parse(duration);
        var durSeconds = dur.ToTotalSeconds();
        if (durSeconds <= 0)
            throw new ArgumentException("Duration must be positive.");
        var result = new List<string>();
        var current = dtStart;
        while (current < dtFinish)
        {
            var next = current.AddSeconds(durSeconds);
            if (next > dtFinish) next = dtFinish;
            result.Add($"{current:yyyy-MM-dd}/{next:yyyy-MM-dd}");
            current = next;
        }
        return result;
    }

    /// <summary>
    /// Normalizes a date/time string (with or without offset) to ISO 8601 with offset (e.g., '2024-05-01T14:30+03:30').
    /// If no offset is present, uses defaultOffset or 'Z'.
    /// </summary>
    /// <param name="value">The date/time string to normalize (e.g., '2024-05-01T14:30+03:30').</param>
    /// <param name="defaultOffset">An optional default offset to use if none is found (e.g., '+03:30', 'Z').</param>
    /// <returns>A normalized ISO 8601 date/time string with offset.</returns>
    public static string NormalizeToIsoWithOffset(this string value, string? defaultOffset = null)
    {
        var (datePart, offset, _) = value.SplitTime();
        var iso = datePart.AutoNormalizeToIsoDateString();
        if (string.IsNullOrEmpty(offset))
            offset = defaultOffset ?? "Z";
        return $"{iso}{offset}";
    }

    /// <summary>
    /// Converts a date/time string (with or without offset) to the target offset, returning ISO 8601 with new offset.
    /// </summary>
    /// <param name="value">The date/time string to convert (e.g., '2024-05-01T14:30+03:30').</param>
    /// <param name="targetOffset">The target offset to convert to (e.g., '+03:30', 'Z').</param>
    /// <returns>A date/time string in the target offset.</returns>
    public static string ToTimeZone(this string value, string targetOffset)
    {
        var (datePart, offset, _) = value.SplitTime();
        var iso = datePart.AutoNormalizeToIsoDateString();
        // Parse the base time as UTC if offset is present, else as local
        DateTime dt;
        if (!string.IsNullOrEmpty(offset) && offset != "Z")
        {
            if (!DateTimeOffset.TryParse($"{iso}{offset}", out var dto))
                throw new FormatException($"Cannot parse date/time with offset: {value}");
            dt = dto.UtcDateTime;
        }
        else if (offset == "Z")
        {
            if (!DateTime.TryParse(iso, out dt))
                throw new FormatException($"Cannot parse date/time: {value}");
        }
        else
        {
            if (!DateTime.TryParse(iso, out dt))
                throw new FormatException($"Cannot parse date/time: {value}");
        }
        // Parse target offset
        TimeSpan target;
        if (targetOffset == "Z")
            target = TimeSpan.Zero;
        else if (!TimeSpan.TryParse(targetOffset, out target))
        {
            // Try parsing as "+03:30" or "-04:00"
            if (!TimeSpan.TryParse(targetOffset.Replace("+", ""), out target))
                throw new FormatException($"Invalid target offset: {targetOffset}");
        }
        var newDt = dt + target;
        var sign = target < TimeSpan.Zero ? "-" : "+";
        var abs = target.Duration();
        var offsetStr = target == TimeSpan.Zero ? "Z" : $"{sign}{abs.Hours:D2}:{abs.Minutes:D2}";
        return $"{newDt:yyyy-MM-ddTHH:mm:ss}{offsetStr}";
    }

    /// <summary>
    /// Extracts the time zone offset from a date/time string (returns '+HH:mm', '-HH:mm', or 'Z').
    /// </summary>
    /// <param name="value">The date/time string to extract offset from (e.g., '2024-05-01T14:30+03:30').</param>
    /// <returns>The offset string (e.g., '+03:30', 'Z').</returns>
    public static string ExtractOffset(this string value)
    {
        var (_, offset, _) = value.SplitTime();
        if (!string.IsNullOrEmpty(offset))
            return offset;
        return "Z";
    }

    /// <summary>
    /// Returns true if the date string is a business day (not a weekend or holiday).
    /// </summary>
    /// <param name="date">The date string to check (e.g., '2024-05-01').</param>
    /// <param name="holidays">An optional list of holiday strings (e.g., '2024-05-01', '2024-05-02').</param>
    /// <param name="weekend">An optional list of days of the week that are weekends (e.g., DayOfWeek.Saturday, DayOfWeek.Sunday).</param>
    /// <returns>True if the date is a business day, otherwise false.</returns>
    public static bool IsBusinessDay(string date, IEnumerable<string> holidays = null, IEnumerable<DayOfWeek> weekend = null)
    {
        holidays ??= Array.Empty<string>();
        weekend ??= new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        if (weekend.Contains(dt.DayOfWeek))
            return false;
        if (holidays.Select(h => h.AutoNormalizeToIsoDateString()).Contains(iso))
            return false;
        return true;
    }

    /// <summary>
    /// Returns true if the date string is in the provided list of holiday strings.
    /// </summary>
    /// <param name="date">The date string to check (e.g., '2024-05-01').</param>
    /// <param name="holidays">An enumerable of holiday strings (e.g., '2024-05-01', '2024-05-02').</param>
    /// <returns>True if the date is a holiday, otherwise false.</returns>
    public static bool IsHoliday(string date, IEnumerable<string> holidays)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        return holidays.Select(h => h.AutoNormalizeToIsoDateString()).Contains(iso);
    }

    /// <summary>
    /// Returns the next business day string after the given date.
    /// </summary>
    /// <param name="date">The date string to increment (e.g., '2024-05-01').</param>
    /// <param name="holidays">An optional list of holiday strings (e.g., '2024-05-01', '2024-05-02').</param>
    /// <param name="weekend">An optional list of days of the week that are weekends (e.g., DayOfWeek.Saturday, DayOfWeek.Sunday).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string NextBusinessDay(string date, IEnumerable<string> holidays = null, IEnumerable<DayOfWeek> weekend = null)
    {
        holidays ??= Array.Empty<string>();
        weekend ??= new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        do
        {
            dt = dt.AddDays(1);
        } while (!IsBusinessDay(dt.ToString("yyyy-MM-dd"), holidays, weekend));
        if (date.IsPersianDateString())
            return dt.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return dt.ToString("yyyy-MM-dd").ToHijriString();
        return dt.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the previous business day string before the given date.
    /// </summary>
    /// <param name="date">The date string to decrement (e.g., '2024-05-01').</param>
    /// <param name="holidays">An optional list of holiday strings (e.g., '2024-05-01', '2024-05-02').</param>
    /// <param name="weekend">An optional list of days of the week that are weekends (e.g., DayOfWeek.Saturday, DayOfWeek.Sunday).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string PrevBusinessDay(string date, IEnumerable<string> holidays = null, IEnumerable<DayOfWeek> weekend = null)
    {
        holidays ??= Array.Empty<string>();
        weekend ??= new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        do
        {
            dt = dt.AddDays(-1);
        } while (!IsBusinessDay(dt.ToString("yyyy-MM-dd"), holidays, weekend));
        if (date.IsPersianDateString())
            return dt.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return dt.ToString("yyyy-MM-dd").ToHijriString();
        return dt.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the start of the week for the given date (default: Monday).
    /// </summary>
    /// <param name="date">The date string to get the start of the week for (e.g., '2024-05-01').</param>
    /// <param name="firstDay">The day of the week to start the week on (default: Monday).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string StartOfWeek(string date, DayOfWeek firstDay = DayOfWeek.Monday)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        int diff = (7 + (dt.DayOfWeek - firstDay)) % 7;
        var start = dt.AddDays(-diff);
        if (date.IsPersianDateString())
            return start.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return start.ToString("yyyy-MM-dd").ToHijriString();
        return start.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the end of the week for the given date (default: Monday as first day).
    /// </summary>
    /// <param name="date">The date string to get the end of the week for (e.g., '2024-05-01').</param>
    /// <param name="firstDay">The day of the week to start the week on (default: Monday).</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string EndOfWeek(string date, DayOfWeek firstDay = DayOfWeek.Monday)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        int diff = (7 - (dt.DayOfWeek - firstDay) - 1) % 7;
        var end = dt.AddDays(diff);
        if (date.IsPersianDateString())
            return end.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return end.ToString("yyyy-MM-dd").ToHijriString();
        return end.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the first day of the month for the given date.
    /// </summary>
    /// <param name="date">The date string to get the start of the month for (e.g., '2024-05-01').</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string StartOfMonth(string date)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var start = new DateTime(dt.Year, dt.Month, 1);
        if (date.IsPersianDateString())
            return start.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return start.ToString("yyyy-MM-dd").ToHijriString();
        return start.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the last day of the month for the given date.
    /// </summary>
    /// <param name="date">The date string to get the end of the month for (e.g., '2024-05-01').</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string EndOfMonth(string date)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var end = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        if (date.IsPersianDateString())
            return end.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return end.ToString("yyyy-MM-dd").ToHijriString();
        return end.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the first day of the year for the given date.
    /// </summary>
    /// <param name="date">The date string to get the start of the year for (e.g., '2024-05-01').</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string StartOfYear(string date)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var start = new DateTime(dt.Year, 1, 1);
        if (date.IsPersianDateString())
            return start.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return start.ToString("yyyy-MM-dd").ToHijriString();
        return start.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns the last day of the year for the given date.
    /// </summary>
    /// <param name="date">The date string to get the end of the year for (e.g., '2024-05-01').</param>
    /// <returns>A date string in the same calendar as the input.</returns>
    public static string EndOfYear(string date)
    {
        var iso = date.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(iso, out var dt))
            throw new FormatException($"Cannot parse date: {date}");
        var end = new DateTime(dt.Year, 12, 31);
        if (date.IsPersianDateString())
            return end.ToString("yyyy-MM-dd").ToPersianString();
        if (date.IsHijriDateString())
            return end.ToString("yyyy-MM-dd").ToHijriString();
        return end.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Generates a list of date strings from an RRULE (e.g., 'FREQ=WEEKLY;BYDAY=MO,WE,FR'), starting from startDate.
    /// Supports FREQ, INTERVAL, BYDAY, COUNT, UNTIL. Calendar-agnostic, string-in, string-out.
    /// </summary>
    /// <param name="rrule">The RRULE string (e.g., 'FREQ=WEEKLY;BYDAY=MO,WE,FR').</param>
    /// <param name="startDate">The start date string (e.g., '2024-05-01').</param>
    /// <param name="endDate">An optional end date string (e.g., '2024-05-05') to limit the sequence.</param>
    /// <param name="count">An optional maximum number of occurrences to generate.</param>
    /// <returns>A list of date strings generated by the RRULE.</returns>
    public static List<string> ParseRRule(string rrule, string startDate, string? endDate = null, int? count = null)
    {
        if (string.IsNullOrWhiteSpace(rrule) || string.IsNullOrWhiteSpace(startDate))
            throw new ArgumentException("RRULE and startDate are required.");
        var rules = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var part in rrule.Split(';'))
        {
            var kv = part.Split('=');
            if (kv.Length == 2)
                rules[kv[0].Trim()] = kv[1].Trim();
        }
        var freq = rules.TryGetValue("FREQ", out var f) ? f.ToUpperInvariant() : "DAILY";
        var interval = rules.TryGetValue("INTERVAL", out var i) && int.TryParse(i, out var iv) ? iv : 1;
        var byday = rules.TryGetValue("BYDAY", out var bd) ? bd.Split(',') : null;
        var until = rules.TryGetValue("UNTIL", out var u) ? u : null;
        var maxCount = rules.TryGetValue("COUNT", out var c) && int.TryParse(c, out var cnt) ? cnt : (count ?? int.MaxValue);
        var result = new List<string>();
        var isoStart = startDate.AutoNormalizeToIsoDateString();
        if (!DateTime.TryParse(isoStart, out var dt))
            throw new FormatException($"Cannot parse start date: {startDate}");
        DateTime? dtEnd = null;
        if (endDate != null)
        {
            var isoEnd = endDate.AutoNormalizeToIsoDateString();
            if (DateTime.TryParse(isoEnd, out var dte))
                dtEnd = dte;
        }
        if (until != null && DateTime.TryParse(until, out var untilDt))
        {
            if (dtEnd == null || untilDt < dtEnd)
                dtEnd = untilDt;
        }
        int added = 0;
        var current = dt;
        while (added < maxCount && (dtEnd == null || current <= dtEnd))
        {
            bool add = true;
            if (byday != null && byday.Length > 0)
            {
                var dayMap = new Dictionary<string, DayOfWeek> {
                    {"MO", DayOfWeek.Monday}, {"TU", DayOfWeek.Tuesday}, {"WE", DayOfWeek.Wednesday},
                    {"TH", DayOfWeek.Thursday}, {"FR", DayOfWeek.Friday}, {"SA", DayOfWeek.Saturday}, {"SU", DayOfWeek.Sunday}
                };
                add = Array.Exists(byday, d => dayMap.TryGetValue(d, out var dow) && dow == current.DayOfWeek);
            }
            if (add)
            {
                result.Add(current.ToString("yyyy-MM-dd"));
                added++;
            }
            switch (freq)
            {
                case "DAILY": current = current.AddDays(interval); break;
                case "WEEKLY": current = current.AddDays(7 * interval); break;
                case "MONTHLY": current = current.AddMonths(interval); break;
                case "YEARLY": current = current.AddYears(interval); break;
                default: throw new NotSupportedException($"FREQ={freq} not supported");
            }
        }
        return result;
    }

    /// <summary>
    /// Bulk normalizes a sequence of date strings to ISO 8601 format. Thread-safe and side-effect free.
    /// </summary>
    /// <param name="inputs">A sequence of date strings to normalize.</param>
    /// <returns>A sequence of normalized ISO 8601 date strings.</returns>
    public static IEnumerable<string> BulkNormalizeToIsoDateString(this IEnumerable<string> inputs)
    {
        if (inputs == null) throw new ArgumentNullException(nameof(inputs));
        foreach (var input in inputs)
        {
            yield return input.AutoNormalizeToIsoDateString();
        }
    }

    /// <summary>
    /// Bulk formats a sequence of date strings using the specified format and optional language. Thread-safe and side-effect free.
    /// </summary>
    /// <param name="inputs">A sequence of date strings to format.</param>
    /// <param name="format">The format string (e.g., 'yyyy-MM-dd').</param>
    /// <param name="language">An optional language code for localization (e.g., 'en', 'fa', 'ar').</param>
    /// <returns>A sequence of formatted date strings.</returns>
    public static IEnumerable<string> BulkFormat(this IEnumerable<string> inputs, string format, string? language = null)
    {
        if (inputs == null) throw new ArgumentNullException(nameof(inputs));
        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(format))
                yield return input;
            else if (format == "human-month")
                yield return input.ToHumanMonth(language ?? "en");
            else if (format == "human-week")
                yield return input.ToHumanWeek(language ?? "en");
            else
                yield return DateTime.Parse(input.AutoNormalizeToIsoDateString()).ToString(format);
        }
    }
}