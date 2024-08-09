using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Hangfire;
using MyStik.TimeTable.Web.Areas.Admin.Models;
using MyStik.TimeTable.Web.Jobs;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "SysAdmin")]
    public class LoggingController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            var n = 250;
            if (id.HasValue)
                n = id.Value;

            var db = new LogDbContext();
            var model = db.Log.OrderByDescending(l => l.Date).Take(n).OrderBy(l => l.Date);

            var success = bool.TryParse(ConfigurationManager.AppSettings["log:BulkDelete"], out var isBulkDelete);
            if (success != true)
            {
                isBulkDelete = false;
            }

            ViewBag.IsBulkDelete = isBulkDelete;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileResult GetLogs()
        {
            var start = new DateTime(2014, 2, 22);

            var db = new LogDbContext();
            var logs = db.Log.Where(log => log.Date >= start).OrderBy(log => log.Date);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Datum;Zeit;Level;Logger;Nachricht");

            writer.Write(Environment.NewLine);
            foreach (var log in logs)
            {
                writer.Write(String.Format("{0};{1};{2};{3};{4}",
                    log.Date.Date.ToShortDateString(),
                    log.Date.TimeOfDay,
                    log.Level,
                    log.Logger,
                    log.Message
                    ));
                writer.Write(Environment.NewLine);

            }

            writer.Flush();
            writer.Dispose();

            return File(ms.GetBuffer(), "text/csv", "Logs.csv");
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetErrors()
        {
            var db = new LogDbContext();
            var model = db.Log.Where(l => l.Level.Equals("ERROR") || l.Level.Equals("FATAL")).OrderByDescending(l => l.Date).Take(250).OrderBy(l => l.Date);

            return View("Index", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMails()
        {
            var db = new LogDbContext();
            var model = db.Log.Where(l => l.Logger.Equals("SendMail")).OrderByDescending(l => l.Date).Take(250).OrderBy(l => l.Date);

            return View("Index", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StartClear()
        {
            ConfigurationManager.AppSettings["log:BulkDelete"] = true.ToString();

            RecurringJob.AddOrUpdate<LoggingJob>("Logging.BulkDelete", x => x.BulkDelete(), Cron.Hourly(15));

            return RedirectToAction("Index");
        }

        public ActionResult StopClear()
        {
            ConfigurationManager.AppSettings["log:BulkDelete"] = false.ToString();

            RecurringJob.RemoveIfExists("Logging.BulkDelete");

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Search(string thread, string logger)
        {
            var db = new LogDbContext();

            List<Log> logs = null;

            if (string.IsNullOrEmpty(thread) && !string.IsNullOrEmpty(logger))
            {
                logs = db.Log.Where(l => l.Logger.Contains(logger)).ToList();
            }
            else if (!string.IsNullOrEmpty(thread) && string.IsNullOrEmpty(logger))
            {
                logs = db.Log.Where(l => l.Thread.Contains(logger)).ToList();
            }
            else if (!string.IsNullOrEmpty(thread) && !string.IsNullOrEmpty(logger))
            {
                logs = db.Log.Where(l => l.Logger.Contains(logger) && l.Thread.Contains(thread)).ToList();
            }
            else
            {
                logs = new List<Log>();
            }

            return PartialView("_LogTable", logs);
        }
    }
}