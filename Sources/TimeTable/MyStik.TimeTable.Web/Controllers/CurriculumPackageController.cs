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
    public class CurriculumPackageController : BaseController
    {
        // GET: CurriculumPackage
        public ActionResult Index(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);
            var model = curr.Packages.ToList();
            return View(model);
        }

        // GET: CurriculumPackage/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumPackage curriculumPackage = Db.CurriculumPackages.Find(id);
            if (curriculumPackage == null)
            {
                return HttpNotFound();
            }
            return View(curriculumPackage);
        }

        // GET: CurriculumPackage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CurriculumPackage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CurriculumPackage curriculumPackage)
        {
            if (ModelState.IsValid)
            {
                curriculumPackage.Id = Guid.NewGuid();
                Db.CurriculumPackages.Add(curriculumPackage);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(curriculumPackage);
        }

        // GET: CurriculumPackage/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumPackage curriculumPackage = Db.CurriculumPackages.Find(id);
            if (curriculumPackage == null)
            {
                return HttpNotFound();
            }
            return View(curriculumPackage);
        }

        // POST: CurriculumPackage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] CurriculumPackage curriculumPackage)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(curriculumPackage).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curriculumPackage);
        }

        // GET: CurriculumPackage/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumPackage curriculumPackage = Db.CurriculumPackages.Find(id);
            if (curriculumPackage == null)
            {
                return HttpNotFound();
            }
            return View(curriculumPackage);
        }

        // POST: CurriculumPackage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CurriculumPackage curriculumPackage = Db.CurriculumPackages.Find(id);
            Db.CurriculumPackages.Remove(curriculumPackage);
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
