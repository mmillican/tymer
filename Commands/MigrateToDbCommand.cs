using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using tymer.Core;
using tymer.Data;

// Ignore Obsolete error on TymerContext
#pragma warning disable 618

namespace tymer.Commands
{
    [Command("migrate-to-db", Description = "Migrate the entries from JSON to SQLite")]
    class MigrateToDbCommand : TymeCommandBase
    {
        protected override int OnExecute(CommandLineApplication app)
        {
            Console.WriteLine("Migrating time entries to DB...");

            try
            {
                using var dbContext = new TymerDbContext();

                Console.WriteLine("Creating database & applying migrations...");
                dbContext.Database.Migrate();

                var jsonContext = new TymerContext();
                var jsonEntries = jsonContext.TimeEntries;

                foreach(var jsonEntry in jsonEntries)
                {
                    dbContext.TimeEntries.Add(jsonEntry);
                }

                dbContext.SaveChanges();

                Console.WriteLine($"--> {jsonEntries.Count} entries migrated to DB");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR migrating to database: ${ex.Message}");
            }

            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            return args;
        }
    }

    #pragma warning restore 618
}
