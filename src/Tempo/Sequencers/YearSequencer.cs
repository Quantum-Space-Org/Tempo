using System;

namespace Quantum.Tempo;

public class YearSequencer : Sequencer<YearSequencer>, Sequencer 
{
    public static YearSequencer New(int current)=>new(current);
    public YearSequencer(int current):base(1,int.MaxValue, current)
    {
    }
    public override string ToString()
    {
        var result= Current().ToString();

        for (int i = 0; i < 4 - result.Length ; i++)
        {
            result = $"0{result}";
        }

        return result;
    }

    public static YearSequencer New(Sequencer<YearSequencer> current) 
        => new YearSequencer(current.Current());
}