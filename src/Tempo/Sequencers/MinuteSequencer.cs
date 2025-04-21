namespace Quantum.Tempo;

public class MinuteSequencer : Sequencer<MinuteSequencer>, Sequencer
{
    public MinuteSequencer(int current):base(0, 59, current)
    {
        
    }

    public static MinuteSequencer New(Sequencer<MinuteSequencer> next) 
        => new MinuteSequencer(next.Current());
}