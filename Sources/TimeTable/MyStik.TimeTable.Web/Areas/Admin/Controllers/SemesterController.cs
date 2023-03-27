using System;
using System.Linq;
using System.Threading;
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

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        /// <summary>
        /// Semester verändern
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = new SemesterEditViewModel();

            var semester = Db.Semesters.SingleOrDefault(s => s.Id == id);

            model.SemesterId = semester.Id;
            model.Name = semester.Name;
            model.StartCourses = semester.StartCourses.ToShortDateString();
            model.EndCourses = semester.EndCourses.ToShortDateString();


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(SemesterEditViewModel model)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);

            semester.Name = model.Name;
            semester.StartCourses = DateTime.Parse(model.StartCourses);
            semester.EndCourses = DateTime.Parse(model.EndCourses);

            Db.SaveChanges();


            return RedirectToAction("Details", new { id = semester.Id });
        }

        /// <summary>
        /// Ein Semesterdatum anlegen
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult CreateDate(Guid id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == id);

            var model = new SemesterDateViewModel
            {
                SemesterId = semester.Id,
                //Start = semester.StartCourses.Date.ToShortDateString(),
                Start = DateTime.Today.ToShortDateString(),
                //End = semester.StartCourses.Date.ToShortDateString(),
                End = DateTime.Today.ToShortDateString(),
                HasCourses = false
            };

            ViewBag.Semester = semester;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateDate(SemesterDateViewModel model)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);



            var semDate = new SemesterDate
            {
                Description = model.Description,
                From = DateTime.Parse(model.Start),
                To = DateTime.Parse(model.End),
                HasCourses = model.HasCourses,
                Semester = semester,

            };

            semester.Dates.Add(semDate);

            Db.SemesterDates.Add(semDate);
            Db.SaveChanges();



            return RedirectToAction("Details", new { id = semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id des Datums, dessen Attributwerte verändert werden solllen</param>
        /// <returns></returns>
        public ActionResult EditDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(s => s.Id == id);
            var semester = Db.Semesters.SingleOrDefault(s => s.Dates.Any(d => d.Id == id));

            var model = new SemesterDateViewModel
            {
                DateId = date.Id,
                SemesterId = semester.Id,
                Start = date.From.ToShortDateString(),
                End = date.To.ToShortDateString(),
                Description = date.Description,
                HasCourses = date.HasCourses
            };

            ViewBag.Semester = semester;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDate(SemesterDateViewModel model)
        {
            // wir müssen uns hier wieder das Objekt aus der Datenbank holen
            // Dazu brauchen wir die Id
            // Daher mmüssen wir die Id schon vorher an das Formular übergeben
            var date = Db.SemesterDates.SingleOrDefault(s => s.Id == model.DateId);
            var semester = Db.Semesters.SingleOrDefault(s => s.Dates.Any(d => d.Id == model.DateId));

            // Jetzt aktualisieren wir die Attributwerte des Objekts aus der Datenbank
            // mit den Attributwerten aus dem Formular
            date.Description = model.Description;
            date.From = DateTime.Parse(model.Start);
            date.To = DateTime.Parse(model.End);
            date.HasCourses = model.HasCourses;
            // Reparatur
            if (date.Semester == null)
            {
                date.Semester = semester;
            }

            // Objekt wurde geändert
            // Jetzt speichern wir es in der Datenbank
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = date.Semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(d => d.Id.Equals(id));

            var semester = date.Semester;

            if (semester != null)
            {
                semester.Dates.Remove(date);
            }

            Db.SemesterDates.Remove(date);

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = semester.Id });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteConfirmed(string id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals(id));

            Db.Semesters.Remove(semester);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}