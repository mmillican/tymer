using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using tymer.Data;

namespace tymer.Commands
{
    [Command("delete", Description = "Delete a specific time entry")]
    class DeleteEntryCommand : TymeCommandBase
    {
        [Argument(0, "id", "ID of entry to delete")]
        public string Id { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            if (!Guid.TryParse(Id, out var entryId))
            {
                Console.WriteLine("Invalid entry ID");
                return base.OnExecute(app);
            }

            using var context = new TymerDbContext();

            var entry = context.TimeEntries.Find(entryId);
            if (entry == null)
            {
                Console.WriteLine("Time entry not found!");
                return base.OnExecute(app);
            }

            context.TimeEntries.Remove(entry);
            context.SaveChanges();

            Console.WriteLine("Time entry deleted!");

            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            return args;
        }
    }
}
