using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using tymer.Core;

namespace tymer.Commands
{
    [Command("list", Description = "List time entries")]
    class ListEntriesCommand : TymeCommandBase
    {
        [Option("-s|--Start", "Start date to view logs for", CommandOptionType.SingleOrNoValue)]
        public DateTime? StartDate { get; set; }
        [Option("-e|--End", "End date to view logs for (inclusive of date)", CommandOptionType.SingleOrNoValue)]
        public DateTime? EndDate { get; set; }

        [Option("-p|--Period", Description = "Time period to view logs for. Does not work with Start/End dates")]
        public string Period { get; set; } = "day";

        [Option("-sb|--SortBy", Description = "What property to sort time entries by in each grouping. Options: start [time, desc], end [time, desc], comments. Default is start time.")]
        public string SortBy { get; set; }

        [Option("-id|--include-id", Description = "Include the entry ID")]
        public bool IncludeEntryId { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            var context = new TymerContext();

            var entries = FilterEntries(context.TimeEntries)
                .OrderByDescending(x => x.StartTime)
                .ThenByDescending(x => x.EndTime);

            if (!entries.Any())
            {
                Console.WriteLine("No time entries recorded for given period.");
                return base.OnExecute(app);
            }

            var hoursSum = entries.Sum(x => x.Duration);

            if (Period.ToLower() == "week" || Period.ToLower() == "month")
            {
                var entriesByDate = entries.GroupBy(x => x.StartTime.Date);

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

        private IEnumerable<TimeEntry> FilterEntries(List<TimeEntry> entries)
        {
            entries = entries
                .Where(x => (!StartDate.HasValue || x.StartTime.Date == StartDate.Value.Date)
                    && (!EndDate.HasValue || x.StartTime.Date == EndDate.Value))
                .ToList();
            
            if (!string.IsNullOrEmpty(Period))
            {
                switch(Period.ToLower())
                {
                    case "day":
                        return entries.Where(x => x.StartTime.Date == DateTime.Now.Date);
                    case "week":
                        return entries.Where(x => x.StartTime.Year == DateTime.Now.Year
                            && GetWeekForDate(x.StartTime) == GetWeekForDate(DateTime.Now));
                    case "month":
                        return entries.Where(x => x.StartTime.Month == DateTime.Now.Month);
                }
            }

            return entries;
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
