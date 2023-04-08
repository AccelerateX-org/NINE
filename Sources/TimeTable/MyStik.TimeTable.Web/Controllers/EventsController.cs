using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class EventsController : BaseController
    {
        // GET: Events
        public ActionResult Index()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel()
            {
                Semester = semester,
                Organiser = org,
            };

            // alle Semester
            model.ActiveSemesters.AddRange(
                Db.Semesters
                    .Where(x => x.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                    .ToList());


            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        public ActionResult Semester(Guid id)
        {
            var semester = SemesterService.GetSemester(id);

            var organiser = GetMyOrganisation();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Events = organiser.Activities.OfType<Event>().Where(x => x.Dates.Any(d => semester.StartCourses <= d.Begin && d.Begin <= semester.EndCourses)).ToList(),
                Organiser = organiser
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }
    }
}