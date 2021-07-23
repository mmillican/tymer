using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using tymer.Core;
using tymer.Data;

namespace tymer.Commands
{
    [Command("migrate-to-db", Description = "Migrate the entries from JSON to SQLite")]
    class MigrateToDbCommand : TymeCommandBase
    {
        protected override int OnExecute(CommandLineApplication app)
        {
            Console.WriteLine("Migrating time entries to DB...");

            using var dbContext = new TymerDbContext();

            var jsonContext = new TymerContext();
            var jsonEntries = jsonContext.TimeEntries;

            foreach(var jsonEntry in jsonEntries)
            {
                dbContext.TimeEntries.Add(jsonEntry);
            }

            dbContext.SaveChanges();

            Console.WriteLine($"--> {jsonEntries.Count} entries migrated to DB");
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            return args;
        }
    }
}
