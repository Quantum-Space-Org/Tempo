using System;
using System.Collections.Generic;
using Quantum.Tempo;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
        {
            PrintHelp();
            return;
        }
        try
        {
            switch (args[0])
            {
                case "parse":
                    if (args.Length < 2) throw new ArgumentException("Missing date argument");
                    Console.WriteLine(args[1].AutoNormalizeToIsoDateString());
                    break;
                case "next":
                    if (args.Length < 2) throw new ArgumentException("Missing date argument");
                    var unit = "day";
                    var count = 1;
                    for (int i = 2; i < args.Length; i++)
                    {
                        if (args[i] == "--unit" && i + 1 < args.Length) unit = args[++i];
                        if (args[i] == "--count" && i + 1 < args.Length) count = int.Parse(args[++i]);
                    }
                    var date = args[1];
                    string result = date;
                    for (int i = 0; i < count; i++)
                    {
                        result = unit switch
                        {
                            "day" => result.NextDate(),
                            "week" => result.NextWeek(),
                            "month" => result.NextMonth(),
                            "year" => result.NextYear(),
                            _ => throw new ArgumentException($"Unknown unit: {unit}")
                        };
                    }
                    Console.WriteLine(result);
                    break;
                case "rrule":
                    if (args.Length < 3) throw new ArgumentException("Usage: rrule <rrule> --start <date> [--count <n>]");
                    string rrule = args[1];
                    string start = null;
                    int? rcount = null;
                    for (int i = 2; i < args.Length; i++)
                    {
                        if (args[i] == "--start" && i + 1 < args.Length) start = args[++i];
                        if (args[i] == "--count" && i + 1 < args.Length) rcount = int.Parse(args[++i]);
                    }
                    if (start == null) throw new ArgumentException("Missing --start argument");
                    var seq = StringExtensions.ParseRRule(rrule, start, null, rcount);
                    foreach (var d in seq) Console.WriteLine(d);
                    break;
                case "fiscal":
                    if (args.Length < 2) throw new ArgumentException("Missing fiscal date argument");
                    var fiscal = new SampleFiscalCalendarProvider();
                    Console.WriteLine(fiscal.ToIso(args[1]));
                    break;
                default:
                    PrintHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            PrintHelp();
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine("Quantum.Tempo CLI Playground");
        Console.WriteLine("Usage:");
        Console.WriteLine("  parse <date>                   Normalize and parse a date string");
        Console.WriteLine("  next <date> --unit <unit> --count <n>   Get the next date(s) by unit (day/week/month/year)");
        Console.WriteLine("  rrule <rrule> --start <date> [--count <n>]   Generate dates from an RRULE");
        Console.WriteLine("  fiscal <fy-date>               Convert fiscal date (FYyyyy-MM-dd) to ISO");
        Console.WriteLine("  --help                         Show this help");
    }
} 