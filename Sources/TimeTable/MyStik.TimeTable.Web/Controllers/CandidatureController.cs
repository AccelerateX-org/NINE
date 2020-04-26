using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CandidatureController : BaseController
    {
        // GET: Candidature
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(Guid id)
        {
            var user = GetCurrentUser();

            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var candidature =
                Db.Candidatures.SingleOrDefault(x => x.Assessment.Id == assessment.Id && x.UserId.Equals(user.Id));

            if (candidature == null)
            {
                candidature = new Candidature
                {
                    Assessment = assessment,
                    Joined = DateTime.Now,
                    UserId = user.Id,
                };

                Db.Candidatures.Add(candidature);
                Db.SaveChanges();
            }

            return RedirectToAction("MyRoom", new {id=candidature.Id});
        }

        public ActionResult MyRoom(Guid id)
        {
            var user = GetCurrentUser();
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);

            if (!user.Id.Equals(candidature.UserId))
            {
                return View("_NoAccess");
            }


            return View(candidature);
        }
    }
}