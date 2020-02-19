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

        [Option("-p|--Period", Description = "Time period to view logs for. Exclusive of Start/End dates")]
        public string Period { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            var context = new TymerContext();

            var entries = FilterEntries(context.TimeEntries)
                .OrderByDescending(x => x.StartTime)
                .ThenByDescending(x => x.EndTime);

            var hoursSum = entries.Sum(x => x.Duration);

            foreach(var entry in entries)
            {
                Console.WriteLine($"{entry.StartTime:yyyy-MM-dd} \t {entry.StartTime:hh:mm tt} \t {entry.EndTime:hh:mm tt} \t {entry.Duration:n2} \t {entry.Comments}");
            }

            Console.WriteLine("----------");
            Console.WriteLine($"Total: {hoursSum:n2}");

            return base.OnExecute(app);
        }

        private IEnumerable<TimeEntry> FilterEntries(List<TimeEntry> entries)
        {
            // TODO: Date filtering

            if (!string.IsNullOrEmpty(Period))
            {
                switch(Period.ToLower())
                {
                    case "day":
                        return entries.Where(x => x.StartTime.Date == DateTime.Now.Date);
                    case "week":
                        return entries.Where(x => GetWeekForDate(x.StartTime) == GetWeekForDate(DateTime.Now));
                    case "month":
                        return entries.Where(x => x.StartTime.Month == DateTime.Now.Month);
                }
            }

            return entries;
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
