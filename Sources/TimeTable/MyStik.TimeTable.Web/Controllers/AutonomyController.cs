using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AutonomyController : BaseController
    {
        // GET: Autonomy
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var aut = org.Autonomy;

            // Fehelende Selbstverwaltung automatisch ergänzen
            if (aut == null)
            {
                aut = new Autonomy();
                aut.Committees = new List<Committee>();

                Db.Autonomy.Add(aut);
                org.Autonomy = aut;

                Db.SaveChanges();
            }


            var model = new OrgAutonomyModel
            {
                Organiser = org,
                Autonomy = aut
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }
    }
}