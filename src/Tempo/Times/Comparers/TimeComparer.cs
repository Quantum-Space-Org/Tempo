using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo.Times.Comparers;

internal class TimeComparer : Comparer<Time>
{
    public static TimeComparer New() => new();

    private TimeComparer()
    {

    }

    public override int Compare(Time x, Time y)
    {
        var xHour = x.GetHour();
        var yHour = y.GetHour();

        if (xHour < yHour) return -1;
        if (xHour > yHour) return 1;


        var xMinute = x.GetMinute();
        var yMinute = y.GetMinute();

        if (xMinute < yMinute) return -1;
        if (xMinute > yMinute) return 1;


        var xSecond = x.GetSecond();
        var ySecond = y.GetSecond();

        if (xSecond < ySecond) return -1;
        if (xSecond > ySecond) return 1;


        return 0;
    }

}