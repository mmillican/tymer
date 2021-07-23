using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using tymer.Data;

namespace tymer.Commands
{
    [Command("list", Description = "List time entries")]
    class ListEntriesCommand : TymeCommandBase
    {
        [Option("-s|--Start <DATE>", Description = "Start date to view logs for")]
        public DateTime? StartDate { get; set; }
        [Option("-e|--End <DATE>", Description = "End date to view logs for (inclusive of date)")]
        public DateTime? EndDate { get; set; }

        [Option("-p|--Period", Description = "Time period to view logs for. Does not work with Start/End dates")]
        public string Period { get; set; } = "day";

        [Option("-sb|--SortBy", Description = "What property to sort time entries by in each grouping. Options: start [time, desc], end [time, desc], comments. Default is start time.")]
        public string SortBy { get; set; }

        [Option("-id|--include-id", Description = "Include the entry ID")]
        public bool IncludeEntryId { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            using var context = new TymerDbContext();

            var entries = context.TimeEntries
                .Where(x => (!StartDate.HasValue || x.StartTime.Date >= StartDate.Value.Date)
                    && (!EndDate.HasValue || x.StartTime.Date <= EndDate.Value.Date));

            // TODO: if you pass in a start/end date for filtering, the periods don't really work.
            if (!string.IsNullOrEmpty(Period))
            {
                entries = Period switch
                {
                    "day" => entries.Where(x => x.StartTime.Date == DateTime.Now.Date),
                    "week" => entries.Where(x => x.StartTime.Year == DateTime.Now.Year
                            && GetWeekForDate(x.StartTime) == GetWeekForDate(DateTime.Now)),
                    "month" => entries.Where(x => x.StartTime.Month == DateTime.Now.Month),
                    "all" => entries,
                    _ => entries // no filtering
                };
            }

            var results = entries
                .OrderByDescending(x => x.StartTime)
                .ThenByDescending(x => x.EndTime)
                .ToList();

            if (!results.Any())
            {
                Console.WriteLine("No time entries recorded for given period.");
                return base.OnExecute(app);
            }

            var hoursSum = results.Sum(x => x.Duration);

            if (Period.ToLower() == "week" || Period.ToLower() == "month")
            {
                var entriesByDate = results.GroupBy(x => x.StartTime.Date);

                foreach(var dateGrp in entriesByDate)
                {
                    Console.WriteLine($"Date: {dateGrp.Key:dddd MMM dd} ({dateGrp.Sum(x => x.Duration):n2})");

                    foreach(var entry in dateGrp)
                    {
                        WriteEntryLine(entry);
                    }

                    Console.WriteLine("");
                }
            }
            else
            {
                foreach (var entry in entries)
                {
                    WriteEntryLine(entry);
                }
            }

            Console.WriteLine("----------");
            Console.WriteLine($"Total: {hoursSum:n2}");

            return base.OnExecute(app);
        }

        private void WriteEntryLine(TimeEntry entry)
        {
            var parts = new List<string>();

            if (IncludeEntryId)
            {
                parts.Add(entry.Id.ToString());
            }

            parts.Add(entry.StartTime.ToString("hh:mm tt"));
            parts.Add(entry.EndTime.ToString("hh:mm tt"));

            parts.Add(entry.Duration.ToString("n2"));

            parts.Add(entry.Comments);

            var line = string.Join("\t", parts);
            Console.WriteLine(line);
        }

        private IEnumerable<TimeEntry> SortEntries(List<TimeEntry> entries)
        {
            return (Period.ToLower()) switch
            {
                "end" => entries.OrderBy(x => x.EndTime),
                "comments" => entries.OrderBy(x => x.Comments),
                _ => entries.OrderBy(x => x.StartTime),
            };
        }

        private static int GetWeekForDate(DateTime date) =>
            CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            return args;
        }
    }
}
