using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using tymer.Core;

namespace tymer.Commands
{
    [Command("log", Description = "Log a new time entry")]
    class LogTimeCommand : TymeCommandBase
    {
        [Argument(0, "start", "Start time")]
        public DateTime StartTime { get; set; }
        
        [Argument(1, "end", "End time")]
        public DateTime EndTime { get; set; }

        [Option("-d <DATE>", Description = "The date for the time entry. Defaults to today.")]
        public DateTime? Date { get; set; }

        [Option("-c|--Comment <COMMENTS>", Description = "Time entry comment")]
        public string Comments { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            if (Date == null)
            {
                Date = DateTime.Now;
            }

            var entry = new TimeEntry
            {
                Id = Guid.NewGuid(),
                StartTime = new DateTime(Date.Value.Year, Date.Value.Month, Date.Value.Day, StartTime.Hour, StartTime.Minute, 0),
                EndTime = new DateTime(Date.Value.Year, Date.Value.Month, Date.Value.Day, EndTime.Hour, EndTime.Minute, 0),
                Comments = Comments
            };

            var context = new TymerContext();

            context.TimeEntries.Add(entry);

            context.SaveEntries();

            Console.WriteLine($"Time entry for {entry.Duration} hrs saved.");
            return base.OnExecute(app);
        }        

        public override List<string> CreateArgs() 
        {
            var args = new List<string>();
            return args;
        }
    }
}
