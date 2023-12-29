using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ScheduleController : BaseController
    {
        // GET: Schedule
        public ActionResult Index(Guid? semId)
        {
            var allPublishedSemester = Db.Semesters.Where(x => x.Groups.Any()).OrderByDescending(s => s.EndCourses).Take(4).ToList();

            Semester semester = null;
            if (semId != null)
            {
                semester = SemesterService.GetSemester(semId);
            }
            else
            {
                semester = Db.Semesters.Where(x => x.Groups.Any()).OrderByDescending(s => s.EndCourses).FirstOrDefault();
            }

            var isStaff = false;

            var curricula = SemesterService.GetActiveCurricula(semester, !isStaff).ToList();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Curricula = curricula
            };

            ViewBag.AllSemester = allPublishedSemester;


            return View(model);
        }

        public ActionResult Details(Guid currId, Guid semId)
        {
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var subjects = Db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == currId)).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();


            var model = new StudyPlanViewModel
            {
                Curriculum = curriculum,
                Semester = semester,
                Modules = modules
            };

            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);

            return View(model);
        }

        [HttpPost]
        public JsonResult CurriculumSelected(Guid curId, Guid semId)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Curriculum", "Schedule", new {curId = curId, semId = semId});
            return Json(new { Url = redirectUrl });
        }

        public ActionResult Curriculum(Guid curId, Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var curriclum = Db.Curricula.SingleOrDefault(x => x.Id == curId);

            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.SemesterGroups.Any(g =>
                    g.Semester.Id == semId && g.CapacityGroup.CurriculumGroup.Curriculum.Id == curId)).ToList();


            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Curriculum = curriclum,
            };

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }

            return View(model);
        }

        [HttpPost]
        public PartialViewResult DialogCourseDescription(Guid id)
        {
            var course = Db.Activities.SingleOrDefault(x => x.Id == id) as Course;

            return PartialView("_DlgDescription", course);
        }

        [HttpPost]
        public PartialViewResult DialogDateList(Guid id)
        {
            var course = Db.Activities.SingleOrDefault(x => x.Id == id) as Course;

            return PartialView("_DlgDateList", course);
        }
    }
}