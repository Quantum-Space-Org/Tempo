using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo.Times.Comparers;

internal class DateComparer : Comparer<Time>
{
    public static DateComparer New() => new();

    private DateComparer()
    {

    }

    public override int Compare(Time x, Time y)
    {
        var xYear = x.GetYear();
        var yYear = y.GetYear();

        if (xYear < yYear) return -1;
        if (xYear > yYear) return 1;


        var xMonth = x.GetMonth();
        var yMonth = y.GetMonth();

        if (xMonth < yMonth) return -1;
        if (xMonth > yMonth) return 1;


        var xDay = x.GetDay();
        var yDay = y.GetDay();

        if (xDay < yDay) return -1;
        if (xDay > yDay) return 1;


        return 0;
    }
}
