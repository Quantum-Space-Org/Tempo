# Quantum.Tempo Features

Welcome to the Quantum.Tempo feature overview! This document lists all major features, with usage examples and links to detailed documentation.

---

## Table of Contents
- [Parsing & Normalization](#parsing--normalization)
- [Formatting & Localization](#formatting--localization)
- [Navigation & Sequencing](#navigation--sequencing)
- [Duration & Arithmetic](#duration--arithmetic)
- [Interval Algebra](#interval-algebra)
- [Recurrence Rules (RRULE)](#recurrence-rules-rrule)
- [Business Day & Holiday Logic](#business-day--holiday-logic)
- [Time Zone Support](#time-zone-support)
- [Fuzzy/Relative Date Parsing](#fuzzyrelative-date-parsing)
- [Week/Month/Year Boundaries](#weekmonthyear-boundaries)
- [Pluggable Calendar System](#pluggable-calendar-system)
- [CLI Playground](#cli-playground)
- [.NET Interop](#net-interop)
- [Advanced Features](#advanced-features)
- [Test Coverage & Documentation](#test-coverage--documentation)
- [Contribution & Packaging](#contribution--packaging)

---

## Parsing & Normalization
- **Auto-detect and normalize date strings (ISO, Persian, Hijri, custom)**
- **Explicit override methods for ambiguous cases**
- **Example:**
  ```csharp
  "1402/02/01".AutoNormalizeToIsoDateString(); // "2023-04-21"
  "01/01/1445".ToGregorianString(); // "2023-07-19"
  ```
- [See details](USAGE.md#string-parsing--normalization)

## Formatting & Localization
- **Convert between ISO, Persian, Hijri, and human-readable formats**
- **Localization:** English, Farsi, Arabic
- **Human-friendly formatting (ToHumanMonth, etc.)**
- **Example:**
  ```csharp
  "2024-05-01".ToPersianString(); // "1403-02-11"
  "2024-05".ToHumanMonth("fa"); // "اردیبهشت 2024"
  ```
- [See details](USAGE.md#formatting)

## Navigation & Sequencing
- **Move forward/backward in time, generate date sequences**
- **Helpers:** NextDate, PrevDate, Sequence, NextWeek, PrevMonth, etc.
- **Example:**
  ```csharp
  "2024-05-01".NextDate();
  "2024-05-01".Sequence("2024-05-05");
  ```
- [See details](USAGE.md#navigation)

## Duration & Arithmetic
- **Parse, format, add, subtract, and compare durations**
- **Supports ISO 8601 and human-friendly formats**
- **Duration sequencing and normalization**
- **Example:**
  ```csharp
  Duration.Add("P1D", "PT2H"); // "P1DT2H"
  "2024-05-01".AddDuration("P3D");
  ```
- [See details](USAGE.md#duration)

## Interval Algebra
- **Create, normalize, and operate on intervals**
- **Union, intersection, difference, duration, Allen’s relations, splitting/chunking**
- **Example:**
  ```csharp
  Interval.Union("2024-01-01/2024-01-10", "2024-01-05/2024-01-15");
  ```
- [See details](USAGE.md#interval-algebra)

## Recurrence Rules (RRULE)
- **Generate recurring date sequences from RRULE strings**
- **Full RFC 5545 RRULE support**
- **Example:**
  ```csharp
  StringExtensions.ParseRRule("FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4", "2024-05-01");
  ```
- [See details](USAGE.md#recurrence-rules-rrule)

## Business Day & Holiday Logic
- **Check if a date is a business day/holiday, navigate business days**
- **Customizable holiday calendars**
- **Example:**
  ```csharp
  StringExtensions.IsBusinessDay("2024-05-03", holidays);
  StringExtensions.NextBusinessDay("2024-05-03", holidays);
  ```
- [See details](USAGE.md#business-dayholiday-logic)

## Time Zone Support
- **Parse, normalize, and convert date/time strings with offsets**
- **Time zone conversion and offset parsing**
- **Example:**
  ```csharp
  "2024-05-01T14:30+03:30".NormalizeToIsoWithOffset();
  "2024-05-01T14:30+03:30".ToTimeZone("Z");
  ```
- [See details](USAGE.md#time-zone-support)

## Fuzzy/Relative Date Parsing
- **Parse "today", "tomorrow", "next Friday", etc.**
- **Natural language date support**
- **Example:**
  ```csharp
  StringExtensions.ParseRelativeDate("next Friday", "2024-05-01");
  ```
- [See details](USAGE.md#fuzzyrelative-date-parsing)

## Week/Month/Year Boundaries
- **Get start/end of week, month, year**
- **Boundary helpers for all supported calendars**
- **Example:**
  ```csharp
  StringExtensions.StartOfWeek("2024-05-01");
  StringExtensions.EndOfMonth("2024-05-01");
  ```
- [See details](USAGE.md#weekmonthyear-boundaries)

## Pluggable Calendar System
- **Implement and register custom calendars (e.g., fiscal, Hebrew, Buddhist)**
- **ICalendarProvider interface and registry**
- **Sample fiscal provider included**
- **Example:**
  ```csharp
  CalendarProviderRegistry.Register(new SampleFiscalCalendarProvider());
  ```
- [See details](USAGE.md#pluggable-calendar-system)

## CLI Playground
- **Try all features interactively from the command line**
- **Supports parsing, navigation, recurrence, and calendar conversion**
- **Example:**
  ```sh
  dotnet run --project src/Tempo.Cli -- parse "1402/02/01"
  dotnet run --project src/Tempo.Cli -- next "2024-05-01" --unit day --count 3
  dotnet run --project src/Tempo.Cli -- rrule "FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4" --start "2024-05-01"
  ```
- [See details](USAGE.md#cli-playground)

## .NET Interop
- **Convert to/from DateTime, DateOnly, TimeOnly, TimeSpan, Calendar**
- **Seamless .NET integration**
- **Example:**
  ```csharp
  DateTime.Parse("2024-05-01");
  "2024-05-01".ToPersianString();
  ```
- [See details](USAGE.md#net-interop)

## Advanced Features
- **Bulk operations:** High-performance, side-effect-free batch normalization/formatting helpers for large datasets.
- **Performance optimization:** Thread-safe, allocation-minimizing APIs for production use.
- **Extensibility:** Pluggable architecture for calendars, holidays, and formatting.
- **Diagnostics & error handling:** Robust error reporting, validation, and diagnostics APIs.
- **Web playground:** (Planned) Web-based interactive playground for trying features online.
- **NuGet packaging:** Distributed as a NuGet package for easy integration.
- **Best practices:** Guidance for robust, maintainable, and extensible date/time code.
- **Example (bulk normalization):**
  ```csharp
  var normalized = StringExtensions.BulkNormalizeToIsoDateString(new[] { "1402/02/01", "2024-05-01" });
  ```
- [See details](USAGE.md#advanced-features--best-practices)

## Test Coverage & Documentation
- **Comprehensive xUnit test suite:** All features are covered by automated tests, ensuring reliability and correctness. (Some legacy test failures may exist for unimplemented or .NET-specific edge cases.)
- **XML doc comments:** All public APIs are documented with XML comments for IntelliSense and discoverability.
- **Professional documentation:** Extensive usage examples, advanced topics, and best practices are provided in [README.md](../README.md) and [USAGE.md](USAGE.md).

## Contribution & Packaging
- **Contribution guidelines:** See [README.md](../README.md) and (if available) CONTRIBUTING.md for how to contribute, report issues, or request features.
- **NuGet package:** Quantum.Tempo is available as a NuGet package for easy installation and updates. See [README.md](../README.md#installation) for details.
- **Open-source ready:** The project is designed for extensibility, clarity, and community contributions.

---

For full details, see [USAGE.md](USAGE.md) and [README.md](../README.md). 

## Interactive REPL Playground

Quantum.Tempo includes an interactive REPL (Read-Eval-Print Loop) playground for exploring all features in real time.

### How to Use the REPL

1. **Run the REPL:**
   ```sh
   dotnet run --project src/Tempo.Repl
   ```
2. **Get Help:**
   Type `help` at the prompt to see all available commands and their descriptions.
3. **Try Features:**
   Enter commands such as:
   - `parse 1402/02/01`
   - `format 2024-05-01`
   - `next 2024-05-01 day`
   - `duration P1D`
   - `interval 2024-01-01/2024-01-10 union 2024-01-05/2024-01-15`
   - `rrule FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4 START=2024-05-01`
   - `businessday 2024-05-03`
   - `timezone 2024-05-01T14:30+03:30 to Z`
   - `fuzzy next Friday`
   - `boundary 2024-05-01`
   - `calendar list`
   - `bulk normalize 1402/02/01,2024-05-01`
   - `diagnostics`
   - `exit`
4. **Exit:**
   Type `exit` or `quit` to leave the REPL.

All major Quantum.Tempo features are accessible via the REPL. Extend or script the REPL for advanced scenarios. 