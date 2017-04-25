using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Models
{
    public class Log
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }

        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

    public class LogDbContext : DbContext
    {
        public DbSet<Log> Log { get; set; }

        public LogDbContext()
            : base()
        {
        }

        public LogDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}