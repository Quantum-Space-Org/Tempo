using System;

namespace Quantum.Tempo;

internal class IsoDateTimeFactory
{
    public static DateTime Create(string value)
    {
        var strings = value.SplitTime();

        var split =
            strings.time.Split("T");

        var strings1 = split[0].Split("-");
        var year = strings1[0];
        var month = strings1.Length > 1 ? strings1[1] : "";
        var day = strings1.Length > 2 ? strings1[2] : "";


        var hour = 00;
        var minute = 00;
        if (split.Length > 1)
        {
            var s = split[1];
            hour = ToInt(s.Split(":")[0]);
            minute = ToInt(s.Split(":")[1]);
        }

        return new DateTime(ToInt(year), ToInt(month), ToInt(day), hour, minute, 0, DateTimeKind.Local);
    }


    private static int ToInt(string year)
    {
        if (year is "" or null)
            return 01;
        return int.Parse(year);
    }

}