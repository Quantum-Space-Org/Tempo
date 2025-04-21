using System;

namespace Quantum.Tempo;

public static class FromIsoExtensions
{
    public static From FromIso(this string value)
    {
        return From.Iso(value);
    }

    public static string NextT(this string value)
    {
        return From.Iso(value).Next();
    }

    public static string ToIsoInterval(this (DateTime start, DateTime finish) dates)
    {
        var start = dates.start.ToIso("year month day");
        var finish = dates.finish.ToIso("year month day");
        return $"{start}/{finish}";
    }
}