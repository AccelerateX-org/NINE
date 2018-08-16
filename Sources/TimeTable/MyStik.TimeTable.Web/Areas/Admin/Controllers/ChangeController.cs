using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Areas.Admin.Models;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Administration der Änderungen
    /// </summary>
    public class ChangeController : BaseController
    {
        /// <summary>
        /// Zeigt die letzten Änderungen (heute)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new ChangeSummaryModel();

            model.Count = Db.DateChanges.Count();
            model.First = Db.DateChanges.OrderBy(x => x.TimeStamp).First().TimeStamp;
            model.Last = Db.DateChanges.OrderByDescending(x => x.TimeStamp).First().TimeStamp;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var model = Db.DateChanges.Where(x => x.Date.Activity.ShortName.Contains(searchString) ||
                                      x.Date.Activity.Name.Contains(searchString) ||
                                      x.Date.Activity.ExternalId.Contains(searchString) ||
                                      x.NotificationContent.Contains(searchString)
                                      ).ToList();
            /*
             * später
            var db = new ApplicationDbContext();

            var userList = db.Users.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.FirstName.Contains(searchString) ||
                    u.LastName.Contains(searchString) ||
                    u.Email.Contains(searchString)).OrderBy(u => u.Registered.Value).ToList();

            foreach (var user in userList)
            {
                
            }

            */


            return PartialView("_ChangeList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAll()
        {
            var changes = Db.DateChanges.OrderBy(x => x.TimeStamp).Take(100).ToList();

            foreach (var change in changes)
            {
                change.Date?.Changes.Remove(change);

                var notifications = change.NotificationStates.ToList();
                foreach (var notification in notifications)
                {
                    Db.NotificationStates.Remove(notification);
                }

                Db.DateChanges.Remove(change);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}