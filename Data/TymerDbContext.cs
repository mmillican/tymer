using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace tymer.Data
{
    public class TymerDbContext : DbContext
    {
        public DbSet<TimeEntry> TimeEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            docsPath = Path.Combine(docsPath, "Tymer/tymer.db");

            options.UseSqlite($"Data Source={docsPath}");
        }
    }
}
