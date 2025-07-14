using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Tempo;

/// <summary>
/// Static registry for pluggable calendar providers.
/// </summary>
public static class CalendarProviderRegistry
{
    private static readonly List<ICalendarProvider> _providers = new();

    /// <summary>
    /// Registers a new calendar provider.
    /// </summary>
    public static void Register(ICalendarProvider provider)
    {
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        _providers.Add(provider);
    }

    /// <summary>
    /// Tries to get a provider that can parse the input string.
    /// </summary>
    public static bool TryGetProvider(string input, out ICalendarProvider provider)
    {
        provider = _providers.FirstOrDefault(p => p.CanParse(input));
        return provider != null;
    }

    /// <summary>
    /// Returns all registered providers.
    /// </summary>
    public static IEnumerable<ICalendarProvider> GetAllProviders() => _providers.AsReadOnly();
} 