namespace Quantum.Tempo.Tests;

public class SequencerTests
{
    [Fact]
    public void nextShouldShiftToStartWhenItReachesTheEnd()
    {
        var sequencer = Sequencer(1, 5);

        Assert.True(1 == sequencer.Current());

        Assert.True(2 == sequencer.Next());
        Assert.True(3 == sequencer.Next().Next());
        Assert.True(4 == sequencer.Next().Next().Next());
        Assert.True(5 == sequencer.Next().Next().Next().Next());

        Assert.True(1 == sequencer.Next().Next().Next().Next().Next());
        Assert.True(2 == sequencer.Next().Next().Next().Next().Next().Next());
    }



    [Fact]
    public void previewShouldShiftToStartWhenItReachesTheStart()
    {
        var sequencer = Sequencer(1, 5);

        Assert.True(1 == sequencer.Current());

        Assert.True(5 == sequencer.Prev());
        Assert.True(4 == sequencer.Prev().Prev());
        Assert.True(3 == sequencer.Prev().Prev().Prev());
        Assert.True(2 == sequencer.Prev().Prev().Prev().Prev());

        Assert.True(1 == sequencer.Prev().Prev().Prev().Prev().Prev());
        Assert.True(5 == sequencer.Prev().Prev().Prev().Prev().Prev().Prev());
    }

    [Fact]
    public void nextTimes()
    {
        var sequencer = Sequencer(1, 5);

        Assert.True(1 == sequencer.Current());

        Assert.True(3 == sequencer.Next(times: 2));
        Assert.True(4 == sequencer.Next(times: 3));
        Assert.True(5 == sequencer.Next(times: 4));
        Assert.True(1 == sequencer.Next(times: 5));
    }

    [Fact]
    public void prevTimes()
    {
        var sequencer = Sequencer(1, 5);

        Assert.True(1 == sequencer.Current());

        Assert.True(5 == sequencer.Prev(times: 1));
        Assert.True(4 == sequencer.Prev(times: 2));
        Assert.True(3 == sequencer.Prev(times: 3));
        Assert.True(2 == sequencer.Prev(times: 4));
        Assert.True(1 == sequencer.Prev(times: 5));
        Assert.True(5 == sequencer.Prev(times: 1));
    }

    private static Sequencer<int> Sequencer(int min, int max, int? current = null)
    {
        return new Sequencer<int>(min, max, current ?? min);
    }
}