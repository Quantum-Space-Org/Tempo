using System;

namespace Quantum.Tempo;

/// <summary>
/// Sample fiscal calendar provider: fiscal year starts in April, format 'FYyyyy-MM-dd'.
/// </summary>
public class SampleFiscalCalendarProvider : ICalendarProvider
{
    /// <summary>
    /// Determines if the input string can be parsed as a fiscal date.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>True if the input starts with "FY", false otherwise.</returns>
    public bool CanParse(string input)
        => input != null && input.StartsWith("FY");

    /// <summary>
    /// Normalizes a fiscal date string by removing the "FY" prefix.
    /// </summary>
    /// <param name="input">The fiscal date string to normalize.</param>
    /// <returns>The normalized string without the "FY" prefix.</returns>
    public string Normalize(string input)
        => input?.Replace("FY", "");

    /// <summary>
    /// Converts a fiscal date string to an ISO 8601 date string.
    /// </summary>
    /// <param name="input">The fiscal date string to convert.</param>
    /// <returns>The ISO 8601 date string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not a valid fiscal date.</exception>
    public string ToIso(string input)
    {
        // FY2024-01-15 means fiscal year 2024, month 1 = April 2023
        if (!CanParse(input)) throw new FormatException("Not a fiscal date");
        var norm = Normalize(input);
        var parts = norm.Split('-');
        int fy = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int day = int.Parse(parts[2]);
        int isoYear = fy - 1 + (month >= 10 ? 1 : 0); // months 1-9 are in fy-1, 10-12 in fy
        int isoMonth = ((month + 2 - 1) % 12) + 1; // fiscal month 1 = April
        if (isoMonth < 1) isoMonth += 12;
        return $"{isoYear:D4}-{isoMonth:D2}-{day:D2}";
    }

    /// <summary>
    /// Converts an ISO 8601 date string to a calendar string.
    /// </summary>
    /// <param name="iso">The ISO 8601 date string to convert.</param>
    /// <param name="calendar">The calendar type to convert to. Only "fiscal" is supported.</param>
    /// <returns>The calendar string.</returns>
    /// <exception cref="NotSupportedException">Thrown if the calendar type is not supported.</exception>
    public string ToCalendarString(string iso, string calendar)
    {
        // Only supports 'fiscal' calendar
        if (calendar.ToLowerInvariant() != "fiscal") throw new NotSupportedException();
        var dt = DateTime.Parse(iso);
        int fy = dt.Month >= 4 ? dt.Year + 1 : dt.Year;
        int fMonth = dt.Month >= 4 ? dt.Month - 3 : dt.Month + 9;
        return $"FY{fy:D4}-{fMonth:D2}-{dt.Day:D2}";
    }

    /// <summary>
    /// Formats a fiscal date string to a specific format.
    /// </summary>
    /// <param name="input">The fiscal date string to format.</param>
    /// <param name="format">The format string (not used in this demo).</param>
    /// <param name="language">The language (not used in this demo).</param>
    /// <returns>The formatted string (in this case, just the normalized input).</returns>
    public string Format(string input, string format, string? language = null)
        => Normalize(input); // For demo, just normalize
} 