namespace Quantum.Tempo;

public class WeekYearSequencer : Sequencer<WeekYearSequencer>, Sequencer
{
    public WeekYearSequencer( int current) : base(1,52, current)
    {
    }

    public static WeekYearSequencer New(Sequencer<WeekYearSequencer> seq) 
        => new(seq.Current());
}