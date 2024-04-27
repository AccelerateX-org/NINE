using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class SchedulingController : BaseController
    {
        // GET: Scheduling
        public ActionResult Label(Guid semId, Guid orgId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);

            var courses = new List<Course>();
            var org = GetOrganiser(orgId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var label = curr.LabelSet.ItemLabels.First();

            courses.AddRange(Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.LabelSet != null &&
                    x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                .ToList());

            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Organiser = org,
                Label = label,
                Courses = courseSummaries
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult GetSegments(Guid orgId, Guid semid)
        {
            var user = GetCurrentUser();
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semid);

            ViewBag.Segments = semester.Dates.Where(x => x.HasCourses && x.Organiser != null && x.Organiser.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.Description,
                Value = c.Id.ToString(),
            });


            return PartialView("_SegmentSelect");
        }

        [HttpPost]
        public PartialViewResult GetLabels(Guid orgId, Guid semid, Guid currId)
        {
            var user = GetCurrentUser();
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semid);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            return PartialView("_LabelList", curr);
        }


    }
}