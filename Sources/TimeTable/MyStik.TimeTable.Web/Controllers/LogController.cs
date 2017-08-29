using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "SysAdmin")]
    public class LogController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var db = new LogDbContext();
            var model = db.Log.OrderByDescending(l => l.Date).Take(250).OrderBy(l => l.Date);

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
        public ActionResult Clear()
        {
            var db = new LogDbContext();

            var history = DateTime.Today.AddYears(-1);

            var oldLogs = db.Log.Where(l => l.Date < history).ToList();

            db.Log.RemoveRange(oldLogs);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Search(string searchText)
        {
            var db = new LogDbContext();

            var oldLogs = db.Log.Where(l => l.Thread.Equals(searchText)).ToList();

            return PartialView("_LogTable", oldLogs);
        }
    }
}