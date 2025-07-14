using System;

namespace Quantum.Tempo;

/// <summary>
/// Interface for pluggable calendar providers (custom calendar parsing/formatting/normalization).
/// </summary>
public interface ICalendarProvider
{
    /// <summary>
    /// Returns true if this provider can parse the input string.
    /// </summary>
    bool CanParse(string input);

    /// <summary>
    /// Normalizes the input string to a canonical form for this calendar.
    /// </summary>
    string Normalize(string input);

    /// <summary>
    /// Converts the input string to ISO 8601 (Gregorian) format.
    /// </summary>
    string ToIso(string input);

    /// <summary>
    /// Converts an ISO 8601 string to this calendar's string format.
    /// </summary>
    string ToCalendarString(string iso, string calendar);

    /// <summary>
    /// Formats the input string using the specified format and optional language.
    /// </summary>
    string Format(string input, string format, string? language = null);
} 