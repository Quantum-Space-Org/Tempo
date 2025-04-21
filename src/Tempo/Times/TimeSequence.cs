using System;
using TimeSequencer.Times;

namespace Quantum.Tempo;

public class TimeSequence
{
    private readonly Time _time;
    public int Count { get; }


    public TimeSequence(Time time, int threshold)
    {
        _time = time;
        Count = threshold;
    }
    
    public Time First() 
        => _time;

    public Time Last() 
        => _time.Next(Count-1);

    public Time nth(int i)
    {
        if (i < 1 || i > Count)
            throw new ArgumentOutOfRangeException(i.ToString());

        return _time.Next(i - 1);
    }

    public Time EndOfMonth()
        => _time.EndOfMonth();
    
    public Time BeginningOfMonth()
        => _time.BeginningOfMonth();
}