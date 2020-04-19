using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AssessmentController : BaseController
    {
        // GET: Assessment
        public ActionResult Index(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentOverviewModel
            {
                Curriculum = curr
            };


            return View(model);
        }

        public ActionResult Details()
        {

            return View();
        }
    }
}