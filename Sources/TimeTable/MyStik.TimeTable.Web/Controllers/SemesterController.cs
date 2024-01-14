using System;
using System.Linq;
using System.Runtime.Serialization.Configuration;
using System.Threading;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterController : BaseController
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid id)
        {
            var org = GetOrganisation(id);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var semesterList = Db.Semesters.Where(x => x.StartCourses > semester.StartCourses)
                .OrderBy(x => x.StartCourses).Take(3).ToList();

            semesterList.Add(semester);

            semesterList.AddRange(
                Db.Semesters.Where(x => x.StartCourses < semester.StartCourses).OrderByDescending(x => x.StartCourses)
                    .Take(3).ToList());

            ViewBag.Organiser = org;
            ViewBag.UserRight = GetUserRight(org);

            return View(semesterList);
        }

        public ActionResult Details(Guid orgId, Guid semId)
        {
            var org = GetOrganiser(orgId);
            var sem = SemesterService.GetSemester(semId);

            var model = new SemesterViewModel()
            {
                Organiser = org,
                Semester = sem
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult CreateDate(Guid orgId, Guid semId)
        {
            var org = GetOrganiser(orgId);
            var sem = SemesterService.GetSemester(semId);

            var model = new SemesterDateViewModel()
            {
                OragniserId = org.Id,
                SemesterId = sem.Id
            };

            ViewBag.Semester = sem;
            ViewBag.Organiser = org;

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateDate(SemesterDateViewModel model)
        {
            var org = GetOrganiser(model.OragniserId);
            var sem = SemesterService.GetSemester(model.SemesterId);

            var startDate = DateTime.Parse(model.Start);
            var endEate = DateTime.Parse(model.End);

            var segment = new SemesterDate
            {
                Semester = sem,
                Description = model.Description,
                From = startDate,
                To = endEate,
                HasCourses = true,
                Organiser = org
            };

            Db.SemesterDates.Add(segment);
            Db.SaveChanges();

            return RedirectToAction("Details", new { orgId = org.Id, semId = sem.Id });
        }

        public ActionResult EditDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(x => x.Id == id);

            var model = new SemesterDateViewModel()
            {
                DateId = date.Id,
                Description = date.Description,
                Start = date.From.ToShortDateString(),
                End = date.To.ToShortDateString(),
            };

            ViewBag.Semester = date.Semester;
            ViewBag.Organiser = date.Organiser;


            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditDate(SemesterDateViewModel model)
        {
            var date = Db.SemesterDates.SingleOrDefault(x => x.Id == model.DateId);

            date.Description = model.Description;
            date.From = DateTime.Parse(model.Start);
            date.To = DateTime.Parse(model.End);

            Db.SaveChanges();

            return RedirectToAction("Details", new { orgId = date.Organiser.Id, semId = date.Semester.Id });
        }

        public ActionResult DeleteDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(x => x.Id == id);
            var org = date.Organiser;
            var sem = date.Semester;

            var courses = Db.Activities.Where(x => x.Segment.Id == date.Id).ToList();
            foreach (var course in courses)
            {
                course.Segment = null;
            }

            Db.SemesterDates.Remove(date);
            Db.SaveChanges();

            return RedirectToAction("Details", new { orgId = org.Id, semId = sem.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DateList(string semGroupId)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semGroupId));

            if (model == null)
            {
                model = Db.Semesters.FirstOrDefault();
            }

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }

        public ActionResult Group(Guid id)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == id);

            return RedirectToAction("CapacityGroup", "Planer",
                new { semId = semGroup.Semester.Id, groupId = semGroup.CapacityGroup.Id });
        }

        public ActionResult Lock(Guid orgId, Guid semId, int state)
        {
            var org = GetOrganiser(orgId);
            var sem = SemesterService.GetSemester(semId);

            var courses = Db.Activities.OfType<Course>().Where(x => x.Organiser.Id == orgId && x.Semester.Id == semId)
                .ToList();

            foreach (var course in courses)
            {
                switch (state)
                {
                    case 1:
                    {
                        course.IsProjected = true;
                        break;
                    }
                    case 2:
                    {
                        course.IsProjected = false;
                        break;
                    }
                    case 3:
                    {
                        course.IsInternal = true;
                        break;
                    }
                    case 4:
                    {
                        course.IsInternal = false;
                        break;
                    }
                    case 5:
                    {
                        course.Occurrence.IsAvailable = false;
                        break;
                    }
                    case 6:
                    {
                        course.Occurrence.IsAvailable = true;
                        break;
                    }
                }
            }


            Db.SaveChanges();

            return RedirectToAction("Details", new { orgId = org.Id, semId = sem.Id });
        }
    }
}