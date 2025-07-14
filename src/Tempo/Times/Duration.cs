using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic; // Added missing import for List

namespace Quantum.Tempo.Times;

/// <summary>
/// Represents a calendar-agnostic duration, with string-based parsing, normalization, and formatting.
/// </summary>
public class Duration
{
    // Internal representation in total seconds for simplicity
    private readonly long _totalSeconds;

    private Duration(long totalSeconds)
    {
        _totalSeconds = totalSeconds;
    }

    /// <summary>
    /// Parses a duration string (ISO 8601 or simple format) and returns a normalized Duration.
    /// </summary>
    /// <param name="input">The duration string (e.g., 'P3D', 'PT2H30M', '3 days', '2:30:00').</param>
    /// <returns>A Duration instance.</returns>
    public static Duration Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty.");

        // Try ISO 8601 duration
        if (input.StartsWith("P", StringComparison.OrdinalIgnoreCase))
        {
            return ParseIso8601(input);
        }

        // Try simple human-friendly format (e.g., '3 days', '2:30:00')
        if (Regex.IsMatch(input, @"^\d+:\d{2}(:\d{2})?$"))
        {
            // Format: HH:mm[:ss]
            var parts = input.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = parts.Length > 2 ? int.Parse(parts[2]) : 0;
            return new Duration(hours * 3600 + minutes * 60 + seconds);
        }
        if (Regex.IsMatch(input, @"^\d+\s*(day|days|hour|hours|minute|minutes|second|seconds)$", RegexOptions.IgnoreCase))
        {
            // Format: '3 days', '2 hours', etc.
            var match = Regex.Match(input, @"^(\d+)\s*(day|days|hour|hours|minute|minutes|second|seconds)$", RegexOptions.IgnoreCase);
            int value = int.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value.ToLowerInvariant();
            int seconds = unit switch
            {
                "day" or "days" => value * 86400,
                "hour" or "hours" => value * 3600,
                "minute" or "minutes" => value * 60,
                "second" or "seconds" => value,
                _ => throw new FormatException($"Unknown duration unit: {unit}")
            };
            return new Duration(seconds);
        }
        throw new FormatException($"Unrecognized duration format: {input}");
    }

    /// <summary>
    /// Returns the canonical ISO 8601 duration string (e.g., 'PT2H30M').
    /// </summary>
    public string ToIsoString()
    {
        long total = _totalSeconds;
        long days = total / 86400;
        total %= 86400;
        long hours = total / 3600;
        total %= 3600;
        long minutes = total / 60;
        long seconds = total % 60;
        string result = "P";
        if (days > 0) result += $"{days}D";
        if (hours > 0 || minutes > 0 || seconds > 0)
        {
            result += "T";
            if (hours > 0) result += $"{hours}H";
            if (minutes > 0) result += $"{minutes}M";
            if (seconds > 0) result += $"{seconds}S";
        }
        if (result == "P") result = "PT0S";
        return result;
    }

    /// <summary>
    /// Returns a human-readable string (e.g., '2 hours, 30 minutes').
    /// </summary>
    public string ToHumanString()
    {
        long total = _totalSeconds;
        long days = total / 86400;
        total %= 86400;
        long hours = total / 3600;
        total %= 3600;
        long minutes = total / 60;
        long seconds = total % 60;
        var parts = new List<string>();
        if (days > 0) parts.Add($"{days} day{(days == 1 ? "" : "s")}");
        if (hours > 0) parts.Add($"{hours} hour{(hours == 1 ? "" : "s")}");
        if (minutes > 0) parts.Add($"{minutes} minute{(minutes == 1 ? "" : "s")}");
        if (seconds > 0) parts.Add($"{seconds} second{(seconds == 1 ? "" : "s")}");
        if (parts.Count == 0) return "0 seconds";
        return string.Join(", ", parts);
    }

    /// <summary>
    /// Parses an ISO 8601 duration string (e.g., 'P3DT2H30M').
    /// </summary>
    private static Duration ParseIso8601(string input)
    {
        // Basic ISO 8601 duration regex (no years/months for simplicity)
        var match = Regex.Match(input, @"^P(?:(\d+)D)?(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?)?$", RegexOptions.IgnoreCase);
        if (!match.Success)
            throw new FormatException($"Invalid ISO 8601 duration: {input}");
        int days = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
        int hours = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
        int minutes = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
        int seconds = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
        long totalSeconds = days * 86400 + hours * 3600 + minutes * 60 + seconds;
        return new Duration(totalSeconds);
    }

    /// <summary>
    /// Returns the total seconds represented by this duration.
    /// </summary>
    public long ToTotalSeconds() => _totalSeconds;

    /// <summary>
    /// Adds two duration strings and returns the normalized ISO 8601 duration string.
    /// </summary>
    public static string Add(string duration1, string duration2)
    {
        var d1 = Parse(duration1);
        var d2 = Parse(duration2);
        return new Duration(d1._totalSeconds + d2._totalSeconds).ToIsoString();
    }

    /// <summary>
    /// Subtracts duration2 from duration1 and returns the normalized ISO 8601 duration string (non-negative, 0 if negative).
    /// </summary>
    public static string Subtract(string duration1, string duration2)
    {
        var d1 = Parse(duration1);
        var d2 = Parse(duration2);
        var diff = d1._totalSeconds - d2._totalSeconds;
        if (diff < 0) diff = 0;
        return new Duration(diff).ToIsoString();
    }

    /// <summary>
    /// Returns true if duration1 is longer than duration2.
    /// </summary>
    public static bool IsLongerThan(string duration1, string duration2)
    {
        var d1 = Parse(duration1);
        var d2 = Parse(duration2);
        return d1._totalSeconds > d2._totalSeconds;
    }

    /// <summary>
    /// Returns true if duration1 is shorter than duration2.
    /// </summary>
    public static bool IsShorterThan(string duration1, string duration2)
    {
        var d1 = Parse(duration1);
        var d2 = Parse(duration2);
        return d1._totalSeconds < d2._totalSeconds;
    }

    /// <summary>
    /// Returns true if duration1 equals duration2.
    /// </summary>
    public static bool Equals(string duration1, string duration2)
    {
        var d1 = Parse(duration1);
        var d2 = Parse(duration2);
        return d1._totalSeconds == d2._totalSeconds;
    }
} 