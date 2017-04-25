using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class QuestionCategoriesController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/QuestionCategories
        public ActionResult Index(string QuestionCategoryShortName, string searchString)
        {
            var ShortNameLst = new List<string>();
            var ShortNameQry = from d in db.Categories
                           orderby d.ShortName
                           select d.ShortName;

           ShortNameLst.AddRange(ShortNameQry.Distinct());
            ViewBag.QuestionCategoryShortName = new SelectList(ShortNameLst);

            var questionCategory = from m in db.Categories
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                questionCategory = questionCategory.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(QuestionCategoryShortName))
            {
                questionCategory = questionCategory.Where(x => x.ShortName == QuestionCategoryShortName);
            }
            return View(questionCategory);
        }

        // GET: Admin/QuestionCategories/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionCategory questionCategory = db.Categories.Find(id);
            if (questionCategory == null)
            {
                return HttpNotFound();
            }
            return View(questionCategory);
        }

        // GET: Admin/QuestionCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/QuestionCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ShortName,Name,Reihenfolge")] QuestionCategory questionCategory)
        {
            if (ModelState.IsValid)
            {
                questionCategory.Id = Guid.NewGuid();
                db.Categories.Add(questionCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionCategory);
        }

        // GET: Admin/QuestionCategories/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionCategory questionCategory = db.Categories.Find(id);
            if (questionCategory == null)
            {
                return HttpNotFound();
            }
            return View(questionCategory);
        }

        // POST: Admin/QuestionCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ShortName,Name,Reihenfolge")] QuestionCategory questionCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionCategory);
        }

        // GET: Admin/QuestionCategories/Delete/5Default1
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionCategory questionCategory = db.Categories.Find(id);
            if (questionCategory == null)
            {
                return HttpNotFound();
            }
            return View(questionCategory);
        }

        // POST: Admin/QuestionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            QuestionCategory questionCategory = db.Categories.Find(id);
            db.Categories.Remove(questionCategory);
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

        public string QuestionCategoriesShortName { get; set; }
    }
}
