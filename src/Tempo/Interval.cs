using System;
using System.Collections.Generic;
using System.Linq;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public static class ArrayExtensions
{
    public static string Nth(this string[] array, int @index)
    {
        return array[@index == 0 ? @index : @index - 1];
    }

    public static string Start(this string[] array)
    {
        return array[0];
    }

    public static string Finish(this string[] array)
    {
        return array[^1];
    }

    public static string[] Take(this string[] array, int @index)
    {
        var result = new string[@index];
        for (var i = 0; i < @index; i++)
        {
            result[i] = array[i];
        }

        return result;
    }
}
public class Interval
{
    public Time? Start { get; set; }
    public Time? Finish { get; set; }

    public Interval() { }
    public Interval(Time start, Time finish)
    {
        Start = start;
        Finish = finish;
    }

    public string[] Sequence()
    {
        List<string> result = new();
        var a = Start;
        while (!a.Equals(Finish))
        {
            result.Add(a.ToString());
            a = a.Next();
        }
        result.Add(Finish.ToString());

        return result.ToArray();
    }


    public string[] ReverseSequence()
    {
        return Sequence().Reverse().ToArray();
    }

    public string nth(int index)
    {
        return Sequence()[index - 1];
    }

    public IntervalRelation Relation(Time value)
    {
        var startDateTime = Start.ToIso().FromIso().ToDateTime();
        var finishDateTime = Finish.ToIso().FromIso().ToDateTime();

        var dateTime = value.ToIso().FromIso().ToDateTime();

        if (startDateTime <= dateTime && dateTime <= finishDateTime) return IntervalRelation.Contains;

        if (startDateTime > dateTime) return IntervalRelation.Before;

        if (dateTime > finishDateTime) return IntervalRelation.Before;

        else throw new NotImplementedException();
    }
}


//https://ics.uci.edu/~alspaugh/cls/shr/allen.html

public enum IntervalRelation
{
    Equal,
    Before,
    After,
    Meets,
    MetBy,
    Starts,
    StartedBy,
    Finishes,
    FinishedBy,
    During,
    Contains,
    Overlaps,
    OverlappedBy
}


//(def converse-relation
//{:equal         :equal
//    :before        :after
//    :after         :before
//    :meets         :met-by
//    :met-by        :meets
//    :starts        :started-by
//    :started-by    :starts
//    :finishes      :finished-by
//    :finished-by   :finishes
//    :during        :contains
//    :contains      :during
//    :overlaps      :overlapped-by
//    :overlapped-by :overlaps})