using System;

namespace Tempo.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Quantum.Tempo REPL Playground");
            Console.WriteLine("Type 'help' for a list of commands, or 'exit' to quit.\n");
            while (true)
            {
                Console.Write("tempo> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                var command = input.Trim().ToLower();
                if (command == "exit" || command == "quit") break;
                if (command == "help")
                {
                    PrintHelp();
                    continue;
                }
                // Parse command and arguments
                var tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var cmd = tokens[0].ToLower();
                var arg = tokens.Length > 1 ? tokens[1] : string.Empty;
                switch (cmd)
                {
                    case "parse":
                        Console.WriteLine($"[parse] Would parse: {arg}");
                        break;
                    case "format":
                        Console.WriteLine($"[format] Would format: {arg}");
                        break;
                    case "next":
                        Console.WriteLine($"[next] Would compute next: {arg}");
                        break;
                    case "prev":
                        Console.WriteLine($"[prev] Would compute previous: {arg}");
                        break;
                    case "duration":
                        Console.WriteLine($"[duration] Would handle duration: {arg}");
                        break;
                    case "interval":
                        Console.WriteLine($"[interval] Would handle interval: {arg}");
                        break;
                    case "rrule":
                        Console.WriteLine($"[rrule] Would handle RRULE: {arg}");
                        break;
                    case "businessday":
                        Console.WriteLine($"[businessday] Would check business day: {arg}");
                        break;
                    case "timezone":
                        Console.WriteLine($"[timezone] Would handle time zone: {arg}");
                        break;
                    case "fuzzy":
                        Console.WriteLine($"[fuzzy] Would parse fuzzy date: {arg}");
                        break;
                    case "boundary":
                        Console.WriteLine($"[boundary] Would compute boundary: {arg}");
                        break;
                    case "calendar":
                        Console.WriteLine($"[calendar] Would handle calendar provider: {arg}");
                        break;
                    case "bulk":
                        Console.WriteLine($"[bulk] Would handle bulk operation: {arg}");
                        break;
                    case "diagnostics":
                        Console.WriteLine($"[diagnostics] Would run diagnostics: {arg}");
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for a list of commands.");
                        break;
                }
            }
            Console.WriteLine("Goodbye!");
        }

        static void PrintHelp()
        {
            Console.WriteLine("\nAvailable commands:");
            Console.WriteLine("  parse <date>         - Parse a date string (any supported calendar)");
            Console.WriteLine("  format <date>        - Format a date in various styles/calendars");
            Console.WriteLine("  next <date> [unit]   - Get the next date/time (e.g., next day, month)");
            Console.WriteLine("  prev <date> [unit]   - Get the previous date/time");
            Console.WriteLine("  duration <args>      - Parse, format, or compute durations");
            Console.WriteLine("  interval <args>      - Interval algebra (union, intersection, etc.)");
            Console.WriteLine("  rrule <args>         - Generate recurring date sequences");
            Console.WriteLine("  businessday <date>   - Check or navigate business days/holidays");
            Console.WriteLine("  timezone <args>      - Parse/convert with time zones");
            Console.WriteLine("  fuzzy <phrase>       - Parse fuzzy/relative dates (e.g., 'next Friday')");
            Console.WriteLine("  boundary <date>      - Get week/month/year boundaries");
            Console.WriteLine("  calendar <args>      - Use or register custom calendar providers");
            Console.WriteLine("  bulk <args>          - Bulk normalization/formatting");
            Console.WriteLine("  diagnostics          - Run diagnostics or show version info");
            Console.WriteLine("  help                 - Show this help message");
            Console.WriteLine("  exit                 - Exit the REPL\n");
        }
    }
}
