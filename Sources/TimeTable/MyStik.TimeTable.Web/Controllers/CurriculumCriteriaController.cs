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
    public class CurriculumCriteriaController : BaseController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid id)
        {
            var req = Db.Requirements.SingleOrDefault(x => x.Id == id);
            var model = req.Criterias.ToList();

            return View(model);
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
            CurriculumCriteria curriculumCriteria = Db.Criterias.Find(id);
            if (curriculumCriteria == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserRight = GetUserRight();
            return View(curriculumCriteria);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Create(Guid? id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id.Value);

            var model = new CurriculumCriteria();
            //model.Curriculum = curr;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curriculumCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurriculumCriteria curriculumCriteria)
        {
            if (ModelState.IsValid)
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == curriculumCriteria.Id);

                curriculumCriteria.Id = Guid.NewGuid();
                //curriculumCriteria.Curriculum = curr;
                Db.Criterias.Add(curriculumCriteria);
                Db.SaveChanges();
                
                return RedirectToAction("Details", "Curricula", new {id=curr.Id});
            }

            return View(curriculumCriteria);
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
            CurriculumCriteria curriculumCriteria = Db.Criterias.Find(id);
            if (curriculumCriteria == null)
            {
                return HttpNotFound();
            }
            return View(curriculumCriteria);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curriculumCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortName,MinECTS,MaxECTS,Option")] CurriculumCriteria curriculumCriteria)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(curriculumCriteria).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curriculumCriteria);
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
            CurriculumCriteria curriculumCriteria = Db.Criterias.Find(id);
            if (curriculumCriteria == null)
            {
                return HttpNotFound();
            }
            return View(curriculumCriteria);
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
            CurriculumCriteria curriculumCriteria = Db.Criterias.Find(id);
            Db.Criterias.Remove(curriculumCriteria);
            Db.SaveChanges();
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
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
