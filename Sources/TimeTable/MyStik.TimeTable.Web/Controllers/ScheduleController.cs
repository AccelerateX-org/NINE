using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    [AllowAnonymous]
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