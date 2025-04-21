using TimeSequencer.Times;

namespace Quantum.Tempo;

public static class IsoNestingExtensions
{
    public static TimeSequence NestMonth(this string time)
            => time.FromIso().ToTime().Nest(DayMonth.Month);

    public static TimeSequence NestDay(this string time)
        => time.FromIso().ToTime().Nest(DayMonth.Day);

    public static TimeSequence Nest(this string time, DayMonth dayMonth)
        => time.FromIso().ToTime().Nest(dayMonth);

    public static Time EnclosingImmediate(this string time)
        => time.FromIso().ToTime().EnclosingImmediate();

    public static Time EnclosingYear(this string time)
        => time.FromIso().ToTime().EnclosingYear();

    public static Time EnclosingMonth(this string time)
        => time.FromIso().ToTime().EnclosingMonth();
}