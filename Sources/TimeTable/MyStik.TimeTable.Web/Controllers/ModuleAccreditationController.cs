using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ModuleAccreditationController : BaseController
    {
        // GET: ModuleAccreditation
        /*
        public ActionResult Index(Guid id)
        {
            var crit = Db.Criterias.SingleOrDefault(x => x.Id == id);
            var model = crit.Accreditations.ToList();

            return View(model);
        }
        */

        // GET: ModuleAccreditation/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleAccreditation moduleAccreditation = Db.Accreditations.Find(id);
            if (moduleAccreditation == null)
            {
                return HttpNotFound();
            }
            return View(moduleAccreditation);
        }

        // GET: ModuleAccreditation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ModuleAccreditation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsMandatory")] ModuleAccreditation moduleAccreditation)
        {
            if (ModelState.IsValid)
            {
                moduleAccreditation.Id = Guid.NewGuid();
                Db.Accreditations.Add(moduleAccreditation);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(moduleAccreditation);
        }

        // GET: ModuleAccreditation/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleAccreditation moduleAccreditation = Db.Accreditations.Find(id);
            if (moduleAccreditation == null)
            {
                return HttpNotFound();
            }
            return View(moduleAccreditation);
        }

        // POST: ModuleAccreditation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsMandatory")] ModuleAccreditation moduleAccreditation)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(moduleAccreditation).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(moduleAccreditation);
        }

        // GET: ModuleAccreditation/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleAccreditation moduleAccreditation = Db.Accreditations.Find(id);
            if (moduleAccreditation == null)
            {
                return HttpNotFound();
            }
            return View(moduleAccreditation);
        }

        // POST: ModuleAccreditation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ModuleAccreditation moduleAccreditation = Db.Accreditations.Find(id);
            Db.Accreditations.Remove(moduleAccreditation);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
