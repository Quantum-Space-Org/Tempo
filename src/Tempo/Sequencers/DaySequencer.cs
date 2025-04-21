using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace Quantum.Tempo;

public class DaySequencer : Sequencer<DaySequencer>, Sequencer
{
    public DaySequencer(int maxValue, int current):base(1, maxValue, current)
    {
        
    }

    public static DaySequencer New(Sequencer<DaySequencer> next)
    {
        return new DaySequencer(next.Max(), next.Current());
    }

    
}