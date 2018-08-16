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
    public class PackageOptionController : BaseController
    {
        

        // GET: PackageOption
        public ActionResult Index(Guid id)
        {
            var package = Db.CurriculumPackages.SingleOrDefault(x => x.Id == id);
            var model = package.Options.ToList();
            return View(model);
        }

        // GET: PackageOption/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageOption packageOption = Db.PackageOptions.Find(id);
            if (packageOption == null)
            {
                return HttpNotFound();
            }
            return View(packageOption);
        }

        // GET: PackageOption/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PackageOption/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] PackageOption packageOption)
        {
            if (ModelState.IsValid)
            {
                packageOption.Id = Guid.NewGuid();
                Db.PackageOptions.Add(packageOption);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(packageOption);
        }

        // GET: PackageOption/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageOption packageOption = Db.PackageOptions.Find(id);
            if (packageOption == null)
            {
                return HttpNotFound();
            }
            return View(packageOption);
        }

        // POST: PackageOption/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] PackageOption packageOption)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(packageOption).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(packageOption);
        }

        // GET: PackageOption/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageOption packageOption = Db.PackageOptions.Find(id);
            if (packageOption == null)
            {
                return HttpNotFound();
            }
            return View(packageOption);
        }

        // POST: PackageOption/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PackageOption packageOption = Db.PackageOptions.Find(id);
            Db.PackageOptions.Remove(packageOption);
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
