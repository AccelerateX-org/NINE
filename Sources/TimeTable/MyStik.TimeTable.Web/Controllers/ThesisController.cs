using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new ThesisCreateModel();

            model.Title = " Neue Abschlussarbeit";
            model.Lecturer = "unbekannt";

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ThesisCreateModel model)
        {
            // Prüfe das Foumlar aif Konsistenz
            if (string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Title", "Muss gegeben sein");
                return View(model);
            }

            // Einbauen in die Datenbank
            var db = new TimeTableDbContext();

            // Abfrage der Datenbank
            // such aus allen Membern (=Dozenten) denjenidgen (einzigen) raus, der diesen Namen hat
            var lecturer = db.Members.SingleOrDefault(m => m.Name.Equals(model.Lecturer));

            /*
            // neues Objekt anlegen
            var t = new Thesis();
            // neues Objekt befüllen
            t.Title = model.Title;
            t.Lecturer = lecturer;
            */

            // neues Objekt zur Datenbank hinzufügen
            //db.Thesis.Add(t);

            // Speichern
            db.SaveChanges();

            // Nach dem Anlegen der Abschlussarbeit zurückkehren zur Startseite
            return RedirectToAction("Index");
        }



    }
}