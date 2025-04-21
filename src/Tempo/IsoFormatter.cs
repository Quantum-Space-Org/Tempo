namespace Quantum.Tempo;

public class IsoFormatter
{
    /// <summary>
    /// "2017-W05"
    /// </summary>
    public const string YearWeek = "xxxx-'W'ww";
    /// <summary>
    /// "2017-W05-2"
    /// </summary>
    public const string YearWeekDay = "xxxx-'W'ww-e";
    /// <summary>
    /// 2017-320
    /// </summary>
    public const string YearDay = "yyyy-DDD";
    /// <summary>
    /// 2017-02-12T20
    /// </summary>
    public const string YearMonthDayTimeHour = "yyyy-MM-dd'T'HH";

    /// <summary>
            /// 2017-02-12T20:32
    /// </summary>
    public const string YearMonthDayTimeHourMinute = "yyyy-MM-dd'T'HH:mm";
    public const string YearMonthDayTimeHourMinuteSecond = "yyyy-MM-dd'T'HH:mm:ss";

    /// <summary>
    /// 2017-02-12
    /// </summary>
    public const string YearMonthDay = "yyyy-MM-dd";

    /// <summary>
    /// 2017-02
    /// </summary>
    public const string YearMonth = "yyyy-MM";
    /// <summary>
    /// 2017
    /// </summary>
    public const string Year = "yyyy";

}