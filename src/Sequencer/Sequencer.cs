using System;
using System.Collections;
using System.Reflection;

namespace Quantum;

public interface Sequencer
{
}

public class Sequencer<T> //where T : Sequencer
                         

{
    private readonly int _minValue;
    private readonly int _maxValue;
    private readonly int _current = 0;
    private Action onReachAtMaxPosition;
    private Action onReachAtMinPosition;
    public Sequencer(int minValue, int maxValue)
        : this(minValue, maxValue, minValue)
    {
    }

    public Sequencer(int minValue, int maxValue, int current)
    {
        _minValue = minValue;
        _maxValue = maxValue;
        _current = current;
    }

    public void SetOnReachAtMaxPosition(Action action)
    {
        this.onReachAtMaxPosition = action;
    }
    public void SetOnReachAtMinPosition(Action action)
    {
        this.onReachAtMinPosition = action;
    }

    public Sequencer<T> Next(int times = 1)
    {
        var result = this;
        for (var i = 0; i < times; i++)
        {
            result = result.Next();
        }

        return result;
    }
    
    public Sequencer<T> Prev(int times = 1)
    {
        var result = this;
        for (var i = 0; i < times; i++)
        {
            result = result.Prev();
        }

        return result;
    }

    protected virtual Sequencer<T> Prev()
    {
        if (_current == _minValue)
        {
            onReachAtMinPosition?.Invoke();

            return Clone(_minValue, _maxValue, _maxValue);
        }

        return Clone(_minValue, _maxValue, _current - 1);
    }

    protected virtual Sequencer<T> Next()
    {
        if (_current == _maxValue)
        {
            onReachAtMaxPosition?.Invoke();

            return Clone(_minValue, _maxValue, _minValue);
        }

        return Clone(_minValue, _maxValue, _current + 1);
    }

    public static bool operator ==(int value, Sequencer<T> that)
    {
        return value == that._current;
    }

    public static bool operator !=(int value, Sequencer<T> that)
    {
        return !(value == that);
    }

    public static bool operator ==(Sequencer<T> @this, int value)
    {
        return value == @this._current;
    }

    public static bool operator !=(Sequencer<T> @this, int value)
    {
        return !(value == @this);
    }

    public static bool operator <(Sequencer <T>@this, int value)
    {
        return @this.Current() < value;
    }

    public static bool operator >(Sequencer<T> @this, int value)
    {
        return @this.Current() > value;
    }


    public static bool operator <(int value, Sequencer<T> @this)
    {
        return value < @this.Current();
    }

    public static bool operator >(int value, Sequencer<T> @this)
    {
        return value > @this.Current();
    }

    public static int operator +(int value, Sequencer<T> @this)
    {
        return @this.Current() + value;
    }

    public static int operator +(Sequencer<T> @this, int value)
    {
        return @this.Current() + value;
    }



    public int Current()
    {
        return _current;
    }


    public override string ToString()
    {
        var result = Current().ToString();

        var l = _maxValue.ToString().Length;

        var resultLength = result.Length;

        for (var i = 0; i < l - resultLength; i++)
        {
            result = $"0{result}";
        }
        return result;
    }

    public int Max() 
        => _maxValue;
    public int Min() 
        => this._minValue;

    private Sequencer<T> Clone(int min, int max, int current)
    {
        return new Sequencer<T>(min, max, current)
        {
            onReachAtMinPosition = this.onReachAtMinPosition,
            onReachAtMaxPosition = this.onReachAtMaxPosition,
        };
    }
    private Sequencer<T> Clone(int min, int max)
    {
        return new Sequencer<T>(min, max, this._current)
        {
            onReachAtMinPosition = this.onReachAtMinPosition,
            onReachAtMaxPosition = this.onReachAtMaxPosition,
        };
    }
}