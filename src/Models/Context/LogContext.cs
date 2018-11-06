using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Room133.Context
{
    public class LogContext : DbContext
    {
        public LogContext()
        {
        }

        public static void EnsureCreated()
        {
            new LogContext().Database.EnsureCreated();
        }

        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Room133db.db");
        }
    }

    

}
