# Quantum.Tempo Usage & Documentation

## Overview
Quantum.Tempo is a .NET library for simple, robust, and calendar-agnostic manipulation of dates, times, intervals, and durations. Its philosophy is to provide string-centric, human-friendly, and ISO-compliant APIs for all major calendar systems, with extensibility for custom calendars.

## Features
- String-based parsing, normalization, and formatting (ISO, Persian, Hijri, custom)
- Auto calendar detection and explicit overrides
- Navigation and sequencing (days, weeks, months, years, durations)
- Duration and interval algebra (arithmetic, comparison, splitting, Allen’s relations)
- Time zone support (offset parsing, conversion)
- Business day/holiday logic (pluggable weekends/holidays)
- Fuzzy/relative date parsing ("today", "next Friday", etc.)
- Week/month/year boundaries (start/end helpers)
- Localization (human-readable output in English, Farsi, Arabic)
- Recurrence rules (RRULE) for iCalendar-style sequences
- Pluggable calendar system (ICalendarProvider, registry, sample provider)
- CLI playground for interactive exploration
- Comprehensive tests and documentation

## Pluggable Calendar System
- Implement `ICalendarProvider` for custom calendars (e.g., fiscal, Hebrew, Buddhist)
- Register with `CalendarProviderRegistry.Register(new MyProvider())`
- Use in normalization/formatting helpers (auto-detected if registered)
- See `SampleFiscalCalendarProvider` for an example

## CLI Playground
- Run `dotnet run --project src/Tempo.Cli -- <command>`
- Commands:
  - `parse <date>`: Normalize and parse a date string
  - `next <date> --unit <unit> --count <n>`: Get the next date(s) by unit (day/week/month/year)
  - `rrule <rrule> --start <date> [--count <n>]`: Generate dates from an RRULE
  - `fiscal <fy-date>`: Convert fiscal date (e.g., `FY2024-01-15`) to ISO

## Localization
- Use `ToHumanMonth`, `ToHumanWeek` with a language code (`"en"`, `"fa"`, `"ar"`)
- Example: `"2024-05".ToHumanMonth("fa") // "اردیبهشت 2024"`

## Recurrence Rules (RRULE)
- Use `ParseRRule(rrule, startDate, endDate, count)` to generate recurring dates
- Example: `ParseRRule("FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4", "2024-05-01")`

## .NET Interop
- Convert Quantum.Tempo strings to/from `DateTime`, `DateOnly`, `TimeOnly`, `TimeSpan`, `Calendar`
- See the documentation for code samples

## Advanced Features & Best Practices
- Add custom recurrence, more calendar providers, or a web playground as needed
- Use XML doc comments for all public APIs for discoverability (IntelliSense)
- Handle errors and edge cases gracefully
- Contribute via PRs, issues, and custom providers

## API Comments & Discoverability
- All public APIs are documented with XML comments
- Use IntelliSense in your IDE for guidance
- See `StringExtensions.cs` for examples

## Contribution
- Fork, clone, and submit PRs for new features or bugfixes
- Add tests and documentation for all contributions

## Tests
- Run `dotnet test` to verify all features

## Contact
- For questions, open an issue or contact the maintainer 