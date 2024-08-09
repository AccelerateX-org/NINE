using System;
using System.Configuration;
using System.Linq;
using log4net;
using MyStik.TimeTable.Web.Areas.Admin.Models;

namespace MyStik.TimeTable.Web.Jobs
{
    public class LoggingJob : BaseJob
    {
        public LoggingJob()
        {
            Logger = LogManager.GetLogger("Logging");
        }

        public void BulkDelete()
        {
            var success = int.TryParse(ConfigurationManager.AppSettings["log:BulkSize"], out var bulkSize);
            if (success != true)
            {
                bulkSize = 1000;
            }

            Logger.DebugFormat("Read Bulk Size: {0}", bulkSize);

            var threshold = DateTime.Today.AddDays(-30);
            Logger.DebugFormat("Threashold: {0}", threshold.ToShortDateString());

            var db = new LogDbContext();

            try
            {
                var start = DateTime.Now;
                var logs = db.Log.Where(x => x.Date < threshold).OrderBy(x => x.Date).Take(bulkSize).ToList();
                var nSize = logs.Count();
                db.Log.RemoveRange(logs);
                db.SaveChanges();
                var stop = DateTime.Now;
                var elapsed = stop - start;
                Logger.InfoFormat("Deleted Logs: {0} in {1}s", nSize, elapsed.Seconds);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Deleted Logs: Error {0}", e.Message);
            }

        }
    }
}