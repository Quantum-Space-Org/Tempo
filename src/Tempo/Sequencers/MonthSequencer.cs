namespace Quantum.Tempo;

public class MonthSequencer : Sequencer<DayOfYearSequencer>, Sequencer
{
    private readonly int _maxDays;

    public MonthSequencer(int current, int maxDays) : base(1, 12, current)
        => _maxDays = maxDays;

    public static MonthSequencer January() => new(1, 31);
    public static MonthSequencer February(bool leapYear) => new(2, leapYear ? 29 : 28);
    public static MonthSequencer March() => new(3, 31);
    public static MonthSequencer April() => new(4, 30);
    public static MonthSequencer May() => new(5, 31);
    public static MonthSequencer June() => new(6, 30);
    public static MonthSequencer July() => new(7, 31);
    public static MonthSequencer August() => new(8, 31);
    public static MonthSequencer September() => new(9, 30);
    public static MonthSequencer October() => new(10, 31);
    public static MonthSequencer November() => new(11, 30);
    public static MonthSequencer December() => new(12, 31);

    public DaySequencer CreateDaySequencer(int current)
        => new DaySequencer(_maxDays , current);

    public DaySequencer CreateDaySequencer()
        => CreateDaySequencer(_maxDays);

    
}