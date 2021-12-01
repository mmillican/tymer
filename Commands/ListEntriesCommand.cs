using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
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
                var weekDates = DateTime.Now.GetWeekStartAndEnd();

                entries = Period switch
                {
                    "day" => entries.Where(x => x.StartTime.Date == DateTime.Now.Date),
                    "week" => entries.Where(x => x.StartTime.Date >= weekDates.Start.Date && x.StartTime.Date <= weekDates.End.Date),
                    "month" => entries.Where(x => x.StartTime.Year == DateTime.Now.Year && x.StartTime.Month == DateTime.Now.Month),
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

            var isGroupedByDate = Period.ToLower() == "week" || Period.ToLower() == "month";

            var table = new Table();

            var columns = new List<TableColumn>();
            if (isGroupedByDate)
            {
                columns.Add(DefineColumn("Date"));
            }

            columns.Add(DefineColumn("Start"));
            columns.Add(DefineColumn("End"));
            columns.Add(DefineColumn("Hours"));
            columns.Add(DefineColumn("Comments"));

            if (IncludeEntryId)
            {
                columns.Add(DefineColumn("Entry ID"));
            }

            table.AddColumns(columns.ToArray());

            if (isGroupedByDate)
            {
                var entriesByDate = results.GroupBy(x => x.StartTime.Date);

                foreach(var dateGrp in entriesByDate)
                {
                    GenerateDateGroupHeaderRow(table, dateGrp.Key.Date, dateGrp.Sum(x => x.Duration));

                    foreach(var entry in dateGrp.OrderBy(x => x.StartTime))
                    {
                        GenerateEntryRow(table, entry, isGroupedByDate);
                    }

                    table.AddEmptyRow(); // Spacer row between groups
                }
            }
            else
            {
                GenerateDateGroupHeaderRow(table, DateTime.Now.Date, entries.ToList().Sum(x => x.Duration));

                foreach (var entry in entries)
                {
                    GenerateEntryRow(table, entry, false);
                }
            }

            AnsiConsole.Render(table);

            return base.OnExecute(app);
        }

        private static TableColumn DefineColumn(string title) =>
            new TableColumn(title)
            {
                Padding = new Padding(2, 0),
            };

        private static IEnumerable<string> AddEmptyColumns(int count)
        {
            var cols = new List<string>();
            for(var idx = 0; idx < count - 1; idx++)
            {
                cols.Add(string.Empty);
            }

            return cols;
        }

        private void GenerateDateGroupHeaderRow(Table table, DateTime date, double groupDuration)
        {
            var columns = new List<string>();

            columns.Add($"[yellow]{date:ddd MMM dd}[/]");

            columns.AddRange(AddEmptyColumns(3));

            columns.Add($"[yellow]{groupDuration:n2}[/]");

            columns.AddRange(AddEmptyColumns(IncludeEntryId ? 2 : 1));

            table.AddRow(columns.ToArray());
        }

        private void GenerateEntryRow(Table table, TimeEntry entry, bool hasDateCol)
        {
             var columns = new List<string>();

            if (hasDateCol)
            {
                columns.Add(""); // empty for the date grouping
            }

            columns.Add(entry.StartTime.ToString("hh:mm tt"));
            columns.Add(entry.EndTime.ToString("hh:mm tt"));
            columns.Add(entry.Duration.ToString("n2"));
            columns.Add(entry.Comments);

            if (IncludeEntryId)
            {
                columns.Add(entry.Id.ToString());
            }

            table.AddRow(columns.ToArray());
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            return args;
        }
    }
}
