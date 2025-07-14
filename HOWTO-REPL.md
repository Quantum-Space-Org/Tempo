# Quantum.Tempo REPL Playground: How-To Guide

## What is the REPL?
The Quantum.Tempo REPL (Read-Eval-Print Loop) is an interactive playground for exploring all features of the Quantum.Tempo date/time framework. It lets you try parsing, formatting, navigation, durations, intervals, recurrence, business days, time zones, fuzzy dates, calendar providers, bulk operations, and more—right from your terminal.

## How to Run the REPL
1. Open a terminal in your project root.
2. Run:
   ```sh
   dotnet run --project src/Tempo.Repl
   ```
3. You’ll see a prompt like:
   ```
   Quantum.Tempo REPL Playground
   Type 'help' for a list of commands, or 'exit' to quit.
   tempo>
   ```

## Getting Help
Type:
```
help
```
This will list all available commands and a brief description for each.

## Example Commands (One for Each Major Feature)
- `parse 1402/02/01` — Parse a Persian date
- `format 2024-05-01` — Format a date in various styles
- `next 2024-05-01 day` — Get the next day
- `prev 2024-05-01 month` — Get the previous month
- `duration P1D` — Parse or format a duration
- `interval 2024-01-01/2024-01-10 union 2024-01-05/2024-01-15` — Interval algebra
- `rrule FREQ=WEEKLY;BYDAY=MO,WE,FR;COUNT=4 START=2024-05-01` — Generate recurring dates
- `businessday 2024-05-03` — Check if a date is a business day
- `timezone 2024-05-01T14:30+03:30 to Z` — Convert time zones
- `fuzzy next Friday` — Parse a fuzzy/relative date
- `boundary 2024-05-01` — Get week/month/year boundaries
- `calendar list` — List or use custom calendar providers
- `bulk normalize 1402/02/01,2024-05-01` — Bulk normalization
- `diagnostics` — Run diagnostics or show version info
- `exit` — Leave the REPL

## Exiting the REPL
Type `exit` or `quit` at the prompt.

## Tips
- You can script the REPL by piping commands from a file or using shell redirection.
- Extend the REPL by adding new commands in `src/Tempo.Repl/Program.cs`.

## Troubleshooting
- If you see build errors, make sure all dependencies are restored: `dotnet restore`
- If the REPL does not start, check your .NET SDK version (should be .NET 6 or later).
- For feature errors, ensure the Quantum.Tempo library is up to date and built.

## More Information
- [docs/FEATURES.md](docs/FEATURES.md#interactive-repl-playground)
- [README.md](README.md) 