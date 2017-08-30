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
    public class BuildingController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Modell bauen
            var model = new List<Models.BuildingViewModel>();

            // Frage die Datenbank ab: alle gebäude
            var buildings = Db.Buildings.ToList();

            // Wandle jedes Gebäude um, und füge es dem Modell hinzu
            foreach (var b in buildings)
            {
                // Das Objekt anlegen, mit dem wir die Anzeige in der Tabelle
                // steuern
                // Das Objekt hat immer den Namen des Gebäudes
                var bm = new Models.BuildingViewModel
                {
                    Name = b.Name,
                };

                // Wir können den Namen nur dann anzeigen, wenn es auch wirklich
                // einer Instiution zugeordnet ist
                if (b.Institution != null)
                {
                    bm.Institution = b.Institution.Name;
                }

                // Das Objekt unserem Modell für die Anzeige der Tabelle zuordnen
                model.Add(bm);
            }


            // Modell an View übergeben
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateBuilding()
        {
            var model = new Models.BuildingCreateModel();
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBuilding(Models.BuildingCreateModel model)
        {
            var institution = Db.Institutions.SingleOrDefault(x => x.Name.Equals(model.Institution));


            if (institution == null)
            {
                // wenn es die Institution nicht gibt, dann anlegen
                institution = new Institution
                {
                    Name = model.Institution,
                };
                Db.Institutions.Add(institution);
                Db.SaveChanges();
            }

            var curr = new Building
            {
                Name = model.Name,
                Institution = institution,
                Address = model.Address
            };
            Db.Buildings.Add(curr);
            Db.SaveChanges();

            /*
             * OH20161228: Adresse als eigene Entität gelöscht
            var address = Db.Addresses.SingleOrDefault(x => x.Name.Equals(model.Address));

            if (address == null)
            {
                // wenn es die Adresse nicht gibt, dann anlegen
                address = new Address
                {
                    Name = model.Address,
                };
                Db.Addresses.Add(address);
                Db.SaveChanges();
            }
            var test = new Building
            {
                Name = model.Name,
                Address = address
            };
            Db.Buildings.Add(test);
            Db.SaveChanges();
            */

            return RedirectToAction("Index");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditBuilding(string id)
        {
            var b = Db.Buildings.FirstOrDefault(x => x.Name.Equals(id));


            var model = new Models.BuildingCreateModel();

            model.Name = b.Name;

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBuilding(Models.BuildingCreateModel model)
        {
            var institution = Db.Institutions.SingleOrDefault(x => x.Name.Equals(model.Institution));

            var b = Db.Buildings.FirstOrDefault(x => x.Name.Equals(model.Name));

            b.Name = model.Name;

            Db.SaveChanges();



            return RedirectToAction("Index");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteBuilding(Guid id)
        {
            // Stockwerksobjekt in der Datenbank suchen
            var building = Db.Buildings.SingleOrDefault(x => x.Id == id);

            if (building != null)
            {
                // Wenn es das Objekt gibt, dann aus der Datenbank entfernen
                Db.Buildings.Remove(building);

                // Aktualisierung der Datenbank übernehmen
                Db.SaveChanges();
            }
            return RedirectToAction("Index");


        }
    }
}
