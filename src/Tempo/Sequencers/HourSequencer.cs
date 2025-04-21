namespace Quantum.Tempo;

public class HourSequencer : Sequencer<HourSequencer>, Sequencer
{
    public HourSequencer(int current):base(1, 24, current)
    {
        
    }

    public static HourSequencer New(Sequencer<HourSequencer> sequencer) 
        => new(sequencer.Current());
}