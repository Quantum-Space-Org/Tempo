using System;

namespace Quantum.Tempo;

public static class DateTimeExtensions
{

    public static string ToIso(this (DateTime start, DateTime finish) intervals)
    {
        return $"{ToIso(intervals.start, "yy MM dd")}/{ToIso(intervals.finish,"yy MM dd")}";
    }

    static readonly string[] formats = { 
        // Basic formats
        "yyyyMMddTHHmmsszzz",
        "yyyyMMddTHHmmsszz",
        "yyyyMMddTHHmmssZ",
        // Extended formats
        "yyyy-MM-ddTHH:mm:sszzz",
        "yyyy-MM-ddTHH:mm:sszz",
        "yyyy-MM-ddTHH:mm:ssZ",
        // All of the above with reduced accuracy
        "yyyyMMddTHHmmzzz",
        "yyyyMMddTHHmmzz",
        "yyyyMMddTHHmmZ",
        "yyyy-MM-ddTHH:mmzzz",
        "yyyy-MM-ddTHH:mmzz",
        "yyyy-MM-ddTHH:mmZ",
        // Accuracy reduced to hours
        "yyyyMMddTHHzzz",
        "yyyyMMddTHHzz",
        "yyyyMMddTHHZ",
        "yyyy-MM-ddTHHzzz",
        "yyyy-MM-ddTHHzz",
        "yyyy-MM-ddTHHZ",
        "yyyy-MM-ddTHHZ"
    };
    

    public static string ToIso(this string value)
    {
        var replace = value.Replace(".", "-");
        var iso = replace
            .Replace("/", "-");

        if(iso.IsAValidIso8601())
            return iso;

        var strings = iso.Split("-");
        var month = int.Parse(strings[1]) > 12 ? strings[2] : strings[1];
        var day = int.Parse(strings[1]) > 12 ? strings[1] : strings[2];

        return $"{strings[0]}-{paddingWithZero(month, 2)}-{paddingWithZero(day, 2)}";

        string paddingWithZero(string value, int count)
        {
            var result = value;
            for (int i = 0; i < count - value.Length; i++)
            {
                result = "0" + result;
            }
            return result ;
        }
    }
        
    public static string ToIso(this DateTime dateTime, string yearMonthDay)
    {
        var strings = yearMonthDay.Split(" ");
        if (strings.Length == 3)
            return $"{dateTime.Year}-{DateTimeMonth(dateTime.Month)}-{DateTimeMonth(dateTime.Day)}";
        if (strings.Length == 2)
            return $"{dateTime.Year}-{DateTimeMonth(dateTime.Month)}";
        if (strings.Length == 1)
            return $"{dateTime.Year}";

        return "";
    }

    private static string DateTimeMonth(int dateTime)
    {
        return
            dateTime>9
                ? dateTime.ToString() 
                : $"0{dateTime}";
    }
}