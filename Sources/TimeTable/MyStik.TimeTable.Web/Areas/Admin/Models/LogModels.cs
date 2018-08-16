using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MyStik.TimeTable.Web.Areas.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Thread { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Exception { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Log> Log { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LogDbContext() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public LogDbContext(string nameOrConnectionString) 
            : base(nameOrConnectionString)
        { 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }

}