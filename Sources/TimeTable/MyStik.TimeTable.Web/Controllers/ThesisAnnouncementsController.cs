using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisAnnouncementsController : Controller
    {
        private TimeTableDbContext db = new TimeTableDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(db.ThesisAnnouncements.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThesisAnnouncement thesisAnnouncement = db.ThesisAnnouncements.Find(id);
            if (thesisAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(thesisAnnouncement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thesisAnnouncement"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Department,Company")] ThesisAnnouncement thesisAnnouncement)
        {
            if (ModelState.IsValid)
            {
                thesisAnnouncement.Id = Guid.NewGuid();
                db.ThesisAnnouncements.Add(thesisAnnouncement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thesisAnnouncement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThesisAnnouncement thesisAnnouncement = db.ThesisAnnouncements.Find(id);
            if (thesisAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(thesisAnnouncement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thesisAnnouncement"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Department,Company")] ThesisAnnouncement thesisAnnouncement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thesisAnnouncement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thesisAnnouncement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThesisAnnouncement thesisAnnouncement = db.ThesisAnnouncements.Find(id);
            if (thesisAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(thesisAnnouncement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ThesisAnnouncement thesisAnnouncement = db.ThesisAnnouncements.Find(id);
            db.ThesisAnnouncements.Remove(thesisAnnouncement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
