# Quantum.Tempo

Quantum.Tempo is a .NET date/time framework focused on simple, string-based, calendar-agnostic manipulation of dates, times, intervals, and durations. It supports multiple calendar systems (Gregorian, Persian/Shamsi, Hijri/Arabic, and custom) and is designed for robust, human-friendly, and ISO-compliant operations.

For a complete list of supported features, see [docs/FEATURES.md](docs/FEATURES.md).

## Features
- String-based API: All operations use and return strings
- Calendar-agnostic: Supports Gregorian, Persian, Hijri, and pluggable calendars
- Navigation, sequencing, and interval algebra
- Duration parsing, formatting, and arithmetic
- Time zone and offset support
- Business day/holiday logic
- Fuzzy/relative date parsing
- Localization (English, Farsi, Arabic)
- Recurrence rules (RRULE)
- Pluggable calendar system (ICalendarProvider)
- CLI playground for interactive use
- Comprehensive tests and documentation

## Usage Examples
```csharp
// Parse and normalize
"1402/02/01".AutoNormalizeToIsoDateString(); // "2023-04-21"
// Next date, week, month
"2024-05-01".NextDate();
"2024-05-01".NextWeek();
// Duration arithmetic
"2024-05-01".AddDuration("P3D");
// Interval algebra
Interval.Union("2024-01-01/2024-01-10", "2024-01-05/2024-01-15");
// Recurrence
StringExtensions.ParseRRule("FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4", "2024-05-01");
// Pluggable calendar
CalendarProviderRegistry.Register(new SampleFiscalCalendarProvider());

// CLI playground
// dotnet run --project src/Tempo.Cli -- parse "1402/02/01"
```

## API Comments & Discoverability
- All public APIs are documented with XML comments for IntelliSense
- See `docs/USAGE.md` for detailed usage and extension

## Roadmap & Contribution
- Advanced recurrence, more calendar providers, web playground, and more
- Contributions welcome! See `CONTRIBUTING.md` and open issues/PRs

## Tests
- Run `dotnet test` to verify all features

## Interactive REPL Playground

Explore all Quantum.Tempo features interactively using the REPL playground:

```sh
dotnet run --project src/Tempo.Repl
```

Type `help` in the REPL for available commands. See [docs/FEATURES.md](docs/FEATURES.md#interactive-repl-playground) for details and [HOWTO-REPL.md](HOWTO-REPL.md) for a full how-to guide.

## License
MIT
