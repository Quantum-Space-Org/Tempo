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

    /// <summary>
    /// Parses an interval string (e.g., '2024-01-01/2024-01-10', '/2024-01-10', '2024-01-01/') into an Interval object.
    /// Supports ISO, Persian, and Hijri date strings.
    /// </summary>
    public static Interval Parse(string intervalString)
    {
        if (string.IsNullOrWhiteSpace(intervalString))
            throw new ArgumentException("Interval string cannot be null or empty.");
        var parts = intervalString.Split('/');
        if (parts.Length == 1)
        {
            var t = TimeFactory.Create(parts[0]);
            return new Interval(t, t);
        }
        Time? start = string.IsNullOrWhiteSpace(parts[0]) ? null : TimeFactory.Create(parts[0]);
        Time? finish = string.IsNullOrWhiteSpace(parts[1]) ? null : TimeFactory.Create(parts[1]);
        return new Interval(start, finish);
    }

    /// <summary>
    /// Returns the normalized interval string in 'start/end' ISO format.
    /// </summary>
    public string ToIntervalString()
    {
        var startStr = Start?.ToIso() ?? "";
        var finishStr = Finish?.ToIso() ?? "";
        return $"{startStr}/{finishStr}";
    }

    /// <summary>
    /// Returns the minimal covering interval (union) of two intervals as a string, or null if disjoint.
    /// </summary>
    public static string? Union(string interval1, string interval2)
    {
        var i1 = Parse(interval1);
        var i2 = Parse(interval2);
        if (i1.Start == null || i2.Start == null || i1.Finish == null || i2.Finish == null)
            return null;
        // Overlap or adjacent
        if (i1.Finish.CompareTo(i2.Start) >= 0 && i2.Finish.CompareTo(i1.Start) >= 0)
        {
            var start = i1.Start.CompareTo(i2.Start) < 0 ? i1.Start : i2.Start;
            var finish = i1.Finish.CompareTo(i2.Finish) > 0 ? i1.Finish : i2.Finish;
            return new Interval(start, finish).ToIntervalString();
        }
        // Disjoint
        return null;
    }

    /// <summary>
    /// Returns the intersection (overlap) of two intervals as a string, or null if none.
    /// </summary>
    public static string? Intersection(string interval1, string interval2)
    {
        var i1 = Parse(interval1);
        var i2 = Parse(interval2);
        if (i1.Start == null || i2.Start == null || i1.Finish == null || i2.Finish == null)
            return null;
        var start = i1.Start.CompareTo(i2.Start) > 0 ? i1.Start : i2.Start;
        var finish = i1.Finish.CompareTo(i2.Finish) < 0 ? i1.Finish : i2.Finish;
        if (start.CompareTo(finish) < 0)
            return new Interval(start, finish).ToIntervalString();
        return null;
    }

    /// <summary>
    /// Returns the part of interval1 not in interval2 as a list of interval strings (may be empty or one/two intervals).
    /// </summary>
    public static List<string> Difference(string interval1, string interval2)
    {
        var i1 = Parse(interval1);
        var i2 = Parse(interval2);
        var results = new List<string>();
        if (i1.Start == null || i2.Start == null || i1.Finish == null || i2.Finish == null)
            return results;
        // No overlap
        if (i1.Finish.CompareTo(i2.Start) <= 0 || i2.Finish.CompareTo(i1.Start) <= 0)
        {
            results.Add(new Interval(i1.Start, i1.Finish).ToIntervalString());
            return results;
        }
        // Full overlap
        if (i2.Start.CompareTo(i1.Start) <= 0 && i2.Finish.CompareTo(i1.Finish) >= 0)
        {
            return results; // empty
        }
        // Left part
        if (i1.Start.CompareTo(i2.Start) < 0)
        {
            var leftFinish = i2.Start;
            if (i1.Start.CompareTo(leftFinish) < 0)
                results.Add(new Interval(i1.Start, leftFinish).ToIntervalString());
        }
        // Right part
        if (i1.Finish.CompareTo(i2.Finish) > 0)
        {
            var rightStart = i2.Finish;
            if (rightStart.CompareTo(i1.Finish) < 0)
                results.Add(new Interval(rightStart, i1.Finish).ToIntervalString());
        }
        return results;
    }

    /// <summary>
    /// Returns the ISO 8601 duration string of the interval.
    /// </summary>
    public static string Duration(string intervalString)
    {
        var i = Parse(intervalString);
        if (i.Start == null || i.Finish == null)
            return "PT0S";
        var start = i.Start.ToDateTime();
        var finish = i.Finish.ToDateTime();
        var duration = finish - start;
        // Convert TimeSpan to ISO 8601 duration
        return TimeSpanToIso8601(duration);
    }

    private static string TimeSpanToIso8601(TimeSpan ts)
    {
        if (ts == TimeSpan.Zero) return "PT0S";
        var sb = new System.Text.StringBuilder();
        sb.Append('P');
        if (ts.Days > 0) sb.Append(ts.Days + "D");
        if (ts.Hours > 0 || ts.Minutes > 0 || ts.Seconds > 0)
        {
            sb.Append('T');
            if (ts.Hours > 0) sb.Append(ts.Hours + "H");
            if (ts.Minutes > 0) sb.Append(ts.Minutes + "M");
            if (ts.Seconds > 0) sb.Append(ts.Seconds + "S");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Returns the Allen's interval relation between two intervals as a string (e.g., 'Before', 'Overlaps').
    /// </summary>
    public static string Relation(string interval1, string interval2)
    {
        var i1 = Parse(interval1);
        var i2 = Parse(interval2);
        var s1 = i1.Start.ToDateTime();
        var e1 = i1.Finish.ToDateTime();
        var s2 = i2.Start.ToDateTime();
        var e2 = i2.Finish.ToDateTime();
        if (e1 < s2) return "Before";
        if (s1 > e2) return "After";
        if (e1 == s2) return "Meets";
        if (s1 == e2) return "MetBy";
        if (s1 == s2 && e1 < e2) return "Starts";
        if (s1 == s2 && e1 > e2) return "StartedBy";
        if (e1 == e2 && s1 > s2) return "Finishes";
        if (e1 == e2 && s1 < s2) return "FinishedBy";
        if (s1 > s2 && e1 < e2) return "During";
        if (s1 < s2 && e1 > e2) return "Contains";
        if (s1 < s2 && e1 > s2 && e1 < e2) return "Overlaps";
        if (s1 > s2 && s1 < e2 && e1 > e2) return "OverlappedBy";
        if (s1 == s2 && e1 == e2) return "Equal";
        return "Unknown";
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