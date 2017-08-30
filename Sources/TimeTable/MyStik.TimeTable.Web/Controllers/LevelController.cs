using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Modell bauen
            var model = new List<Models.LevelViewModel>();

            // Frage die Datenbank ab: alle Stockwerke
            var levels = Db.Levels.ToList();

            // Wandle jedes Stockwerk um, und füge es dem Modell hinzu
            foreach (var l in levels)
            {
                // Das Objekt anlegen, mit dem wir die Anzeige in der Tabelle
                // steuern
                // Das Objekt hat immer den Namen des Stockwerkes
                var lm = new Models.LevelViewModel
                {
                    Level = l,
                    Stockwerke = l.Stockwerke,
                };

                // Wir können den Namen nur dann anzeigen, wenn es auch wirklich
                // einer Instiution zugeordnet ist
                /*
                if (l.Plan != null)
                {
                    lm.Plan = l.Plan.Stockwerke;
                }
                */
                // Das Objekt unserem Modell für die Anzeige der Tabelle zuordnen
                model.Add(lm);
            }


            // Modell an View übergeben
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLevel()
        {
            var model = new Models.LevelCreateModel();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateLevel(Models.LevelCreateModel model)
        {
            var stockwerk = Db.Levels.SingleOrDefault(x => x.Stockwerke.Equals(model.Stockwerk));


            if (stockwerk == null)
            {
                // wenn es das Stockwerk nicht gibt, dann anlegen
                stockwerk = new Level
                {
                    Stockwerke = model.Stockwerk,
                };
                Db.Levels.Add(stockwerk);
                Db.SaveChanges();
            }

            var curr = new Level
            {
                Stockwerke = model.Stockwerk,
                Plan = model.Plan,
                Rooms = model.Räume,
            };
            Db.Levels.Add(curr);
            Db.SaveChanges();

    
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditLevel(Models.LevelCreateModel model)
        {
            var stockwerk = Db.Levels.SingleOrDefault(x => x.Stockwerke.Equals(model.Stockwerk));

            var b = Db.Levels.FirstOrDefault(x => x.Stockwerke.Equals(model.Stockwerk));

            b.Stockwerke = model.Stockwerk;

            Db.SaveChanges();



            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteLevel(Guid id)
        {
            // Stockwerksobjekt in der Datenbank suchen
            var level = Db.Levels.SingleOrDefault(x => x.Id == id);

            if (level != null)
            {
                // Wenn es das Objekt gibt, dann aus der Datenbank entfernen
                Db.Levels.Remove(level);

                // Aktualisierung der Datenbank übernehmen
                Db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
