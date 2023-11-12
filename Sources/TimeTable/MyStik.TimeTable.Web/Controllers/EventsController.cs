using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    [AllowAnonymous]

    public class EventsController : BaseController
    {
        // GET: Events
        public ActionResult Index()
        {
            var model = new HomeViewModel();

            var allPublishedSemester =
                Db.Activities.OfType<Course>().Where(x => x.Semester != null).Select(x => x.Semester).Distinct()
                    .OrderByDescending(s => s.EndCourses).Take(4).ToList();

            foreach (var semester in allPublishedSemester)
            {
                var activeOrgs = SemesterService.GetActiveEventOrganiser(semester);

                var semModel = new SemesterActiveViewModel
                {
                    Semester = semester,
                    Organisers = activeOrgs.ToList()
                };

                model.ActiveSemester.Add(semModel);
            }

            return View(model);
        }

        public ActionResult Semester(Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);

            var activeOrgs = SemesterService.GetActiveEventOrganiser(semester);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organisers = activeOrgs
            };

            return View(model);
        }

        public ActionResult Organiser(Guid? semId, Guid orgId)
        {
            var semester = semId.HasValue ? SemesterService.GetSemester(semId) : SemesterService.GetSemester(DateTime.Today);

            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var ev1 = org.Activities.OfType<Event>().Where(e => e.Dates.Any(
                d => semester.StartCourses <= d.Begin && d.Begin <= semester.EndCourses)).ToList();

            var ev2 = Db.Activities.OfType<Event>().Where(c =>
                    c.Semester.Id == semester.Id &&
                    c.Organiser.Id == org.Id)
                .OrderBy(c => c.ShortName).ToList();

            var events = ev1;
            events.AddRange(ev2);
            events = events.Distinct().ToList();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Events = events,
                Organiser = org
            };

            return View(model);
        }


    }
}