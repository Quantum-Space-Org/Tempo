namespace Quantum.Tempo;

public class SecondSequencer : Sequencer<SecondSequencer>, Sequencer
{
    public SecondSequencer(int current):base(0, 59, current)
    {
        
    }

    public static SecondSequencer New(Sequencer<SecondSequencer> next) => new(next.Current());
}