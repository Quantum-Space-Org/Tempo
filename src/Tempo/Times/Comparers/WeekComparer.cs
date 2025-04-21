using System;
using System.Collections.Generic;
using TimeSequencer.Times;

namespace Quantum.Tempo.Times.Comparers;

internal class WeekComparer : Comparer<Time>
{
    public static WeekComparer New() => new();

    private WeekComparer()
    {

    }


    public override int Compare(Time x, Time y)
    {
        throw new NotImplementedException();
    }
}
