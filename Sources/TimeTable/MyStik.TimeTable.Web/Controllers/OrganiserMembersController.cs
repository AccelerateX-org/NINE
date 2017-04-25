using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class OrganiserMembersController : Controller
    {
        private TimeTableDbContext db = new TimeTableDbContext();

        // GET: OrganiserMembers
        public ActionResult Index()
        {
            return View(db.Members.ToList());
        }

        // GET: OrganiserMembers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        // GET: OrganiserMembers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrganiserMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                organiserMember.Id = Guid.NewGuid();
                db.Members.Add(organiserMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organiserMember);
        }

        // GET: OrganiserMembers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        // POST: OrganiserMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organiserMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organiserMember);
        }

        // GET: OrganiserMembers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        // POST: OrganiserMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrganiserMember organiserMember = db.Members.Find(id);
            db.Members.Remove(organiserMember);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
