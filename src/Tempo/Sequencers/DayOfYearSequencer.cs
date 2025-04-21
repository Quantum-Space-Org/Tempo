namespace Quantum.Tempo;

public class DayOfYearSequencer : Sequencer<DayOfYearSequencer>, Sequencer
{
    public DayOfYearSequencer(int max, int current):base(1, max, current)
    {
        
    }

    public override string ToString()
    {
        var result = Current().ToString();

        for (int i = 0; i < 3 - result.Length; i++)
        {
            result = $"0{result}";
        }
        return result;
    }
}