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
    public class CurriculumRequirementController : BaseController
    {
        // GET: CurriculumRequirement
        public ActionResult Index(Guid id)
        {
            var option = Db.PackageOptions.SingleOrDefault(x => x.Id == id);
            var model = option.Requirements.ToList();
            return View(model);
        }

        // GET: CurriculumRequirement/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumRequirement curriculumRequirement = Db.Requirements.Find(id);
            if (curriculumRequirement == null)
            {
                return HttpNotFound();
            }
            return View(curriculumRequirement);
        }

        // GET: CurriculumRequirement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CurriculumRequirement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ShortName,ECTS")] CurriculumRequirement curriculumRequirement)
        {
            if (ModelState.IsValid)
            {
                curriculumRequirement.Id = Guid.NewGuid();
                Db.Requirements.Add(curriculumRequirement);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(curriculumRequirement);
        }

        // GET: CurriculumRequirement/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumRequirement curriculumRequirement = Db.Requirements.Find(id);
            if (curriculumRequirement == null)
            {
                return HttpNotFound();
            }
            return View(curriculumRequirement);
        }

        // POST: CurriculumRequirement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortName,ECTS")] CurriculumRequirement curriculumRequirement)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(curriculumRequirement).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curriculumRequirement);
        }

        // GET: CurriculumRequirement/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurriculumRequirement curriculumRequirement = Db.Requirements.Find(id);
            if (curriculumRequirement == null)
            {
                return HttpNotFound();
            }
            return View(curriculumRequirement);
        }

        // POST: CurriculumRequirement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CurriculumRequirement curriculumRequirement = Db.Requirements.Find(id);
            Db.Requirements.Remove(curriculumRequirement);
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
