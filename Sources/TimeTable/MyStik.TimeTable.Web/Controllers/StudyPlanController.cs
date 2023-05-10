using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudyPlanController : BaseController
    {
        // GET: StudyPlan
        public ActionResult Details(Guid currId, Guid semId)
        {
            var modules = Db.CurriculumModules.Where(x =>
                x.Accreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == currId)).ToList();

            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new StudyPlanViewModel
            {
                Curriculum = curriculum,
                Semester = semester,
                Modules = modules
            };

            return View(model);
        }
    }
}