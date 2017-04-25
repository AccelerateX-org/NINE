using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class PlanerController : BaseController
    {
        private readonly TimeTableInfoService _service = new TimeTableInfoService();

        // GET: Planer
        public ActionResult Index()
        {
            var user = AppUser;


            var progs = _service.GetCurriculums();
            ViewBag.Curriculums = progs;
            ViewBag.Semester = GetSemester();


            ViewBag.Faculties = Db.Organisers.Select(f => new SelectListItem
            {
                Text = f.ShortName,
                Value = f.ShortName,
            });

            /*
            ViewBag.Semesters = Db.Semesters.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name
            });
             */

            ViewBag.Semesters = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = GetSemester().Name,
                    Value = GetSemester().Name
                }
            };


            ViewBag.Curricula = Db.Curricula.Where(c => c.Organiser.ShortName.Equals("FK 09")).Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.Id.ToString(),
            });


            var curr = Db.Curricula.First();

            if (curr != null)
            {
                var semester = GetSemester();

                var semesterGroups = Db.SemesterGroups.Where(g =>
                    g.Semester.Id == semester.Id &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                var semGroups = semesterGroups.Select(semGroup => new SelectListItem
                {
                    Text = semGroup.FullName,
                    Value = semGroup.Id.ToString()
                }).ToList();

                ViewBag.Groups = semGroups;
            }




            GroupSelectionViewModel model = new GroupSelectionViewModel
            {
                Faculty = "FK 09",
                Curriculum = curr.ShortName,
                Group = "",
                Semester = GetSemester().Name
            };

            ViewBag.UserRight = GetUserRight();


            return View(model);
        }
    }
}