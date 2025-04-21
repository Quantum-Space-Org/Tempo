namespace Quantum.Tempo;

public class DayWeekSequencer : Sequencer<DayWeekSequencer>, Sequencer
{
    public DayWeekSequencer( int current) : base(1,7, current)
    {
    }
    
}