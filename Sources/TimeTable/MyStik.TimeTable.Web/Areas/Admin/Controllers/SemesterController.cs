using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = Db.Semesters.OrderByDescending(x => x.StartCourses).ToList();
            return View(model);
        }

        /// <summary>
        /// Ein Semester anlegen
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new SemesterCreateViewModel();


            if (DateTime.Today.Month >= 3 && DateTime.Today.Month < 10)
            {
                var date = new DateTime(DateTime.Today.Year, 10, 1);
                // heute liegt im SS => das nächste ist das WS
                model.Name = string.Format("WS{0}", DateTime.Today.Year - 2000);
                model.StartCourses = date.Date.ToShortDateString();
                model.EndCourses = date.AddDays(100).Date.ToShortDateString();
            }
            else
            {
                // offenbar im WS => das nächste ist das SS
                if (DateTime.Today.Month < 3) // "dieses Jahr"
                {
                    model.Name = string.Format("SS{0}", DateTime.Today.Year - 2000);
                }
                else
                {
                    // nächstes Jahr
                    model.Name = string.Format("SS{0}", DateTime.Today.Year - 1999);
                }
                var date = new DateTime(DateTime.Today.Year, 3, 15);
                model.StartCourses = date.Date.ToShortDateString();
                model.EndCourses = date.AddDays(100).Date.ToShortDateString();
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(SemesterCreateViewModel model)
        {
            // Vorbedingungen geprüft
            if (!ModelState.IsValid)
                return View(model);

            var from = DateTime.Parse(model.StartCourses);
            var to = DateTime.Parse(model.EndCourses);


            if (from >= to)
            {
                ModelState.AddModelError("StartCourses", "Vorlesungsbeginn liegt später als Vorlesungsende");
                ModelState.AddModelError("EndCourses", "Vorlesungsende liegt früher als Vorlesungsbeginn");
            }

            var semExists = Db.Semesters.Any(s => s.Name.ToUpper().Equals(model.Name.ToUpper()));
            if (semExists)
            {
                ModelState.AddModelError("Name", "Ein Semester mit diesem Namen existiert bereits");
            }

            /*
            semExists = Db.Semesters.SingleOrDefault(s => s.EndCourses >= from);
            if (semExists != null)
            {
                ModelState.AddModelError("StartCourses",
                    string.Format("Überschneidung mit Vorlesungsende von Semester {0}: {1}", semExists.Name,
                        semExists.EndCourses));
            }
            */

            // Nachbedingunen geprüft
            if (ModelState.IsValid)
            {
                // wenn alles ok, dann einen Redirect auf Index
                var sem = Db.Semesters.Add(new Semester
                {
                    Name = model.Name,
                    StartCourses = from,
                    EndCourses = to,
                });
                Db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


    }
}