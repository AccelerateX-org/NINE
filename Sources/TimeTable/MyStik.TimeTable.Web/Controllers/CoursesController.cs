using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CoursesController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Semester semester;
            if (id.HasValue)
            {
                semester = GetSemester(id.Value);
            }
            else
            {
                semester = GetSemester();
            }

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var nextSemester = new SemesterService().GetNextSemester(semester);
            if (nextSemester != null && nextSemester.Groups.Any())
            {
                ViewBag.NextSemester = nextSemester;
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var semester = GetSemester();

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var coursePage =
                Db.Activities.OfType<Course>().Where(c =>
                c.SemesterGroups.Any(g =>
                    g.Semester.Id == semester.Id &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                ).OrderBy(c => c.ShortName).ToList();


            ViewBag.CoursePage = coursePage;

            foreach (var course in coursePage)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                var summary = new CourseSummaryModel();
                summary.Course = course;
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Rooms.AddRange(rooms);


                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                model.Courses.Add(summary);
            }

            // Benutzerrechte
            ViewBag.UserRight = userRight;

            return View(model);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult NoGroups()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var semester = GetSemester();

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var coursePage =
                Db.Activities.OfType<Course>().Where(c =>
                c.Organiser.Id == organiser.Id &&    
                !c.SemesterGroups.Any()
                ).OrderBy(c => c.ShortName).ToList();


            ViewBag.CoursePage = coursePage;

            foreach (var course in coursePage)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                var summary = new CourseSummaryModel();
                summary.Course = course;
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Rooms.AddRange(rooms);


                var days = (from occ in course.Dates
                    select
                    new
                    {
                        Day = occ.Begin.DayOfWeek,
                        Begin = occ.Begin.TimeOfDay,
                        End = occ.End.TimeOfDay,
                    }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                model.Courses.Add(summary);
            }

            // Benutzerrechte
            ViewBag.UserRight = userRight;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Enable(Guid id)
        {
            var semester = GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups =
                Db.SemesterGroups
                .Where(x => 
                    x.Semester.Id == id &&
                    x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)
                .ToList();

            foreach (var semesterGroup in allGroups)
            {
                semesterGroup.IsAvailable = true;
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Disable(Guid id)
        {
            var semester = GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups =
                Db.SemesterGroups
                .Where(x =>
                    x.Semester.Id == id &&
                    x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)
                .ToList();

            foreach (var semesterGroup in allGroups)
            {
                semesterGroup.IsAvailable = false;
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InitGroups(Guid id)
        {
            var semester = GetSemester(id);
            var org = GetMyOrganisation();

            if (semester == null)
                return RedirectToAction("Index");

            var isWS = semester.Name.StartsWith("WS");

            // Alle Curricula durchgehen
            foreach (var curriculum in Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList())
            {
                foreach (var curriculumGroup in curriculum.CurriculumGroups.ToList())
                {
                    foreach (var capacityGroup in curriculumGroup.CapacityGroups.ToList())
                    {
                        if ((capacityGroup.InWS && isWS) || (capacityGroup.InSS && !isWS))
                        {
                            var exist = semester.Groups.Any(g => g.CapacityGroup.Id == capacityGroup.Id);

                            if (!exist)
                            {
                                var semGroup = new SemesterGroup
                                {
                                    CapacityGroup = capacityGroup,
                                    CurriculumGroup = capacityGroup.CurriculumGroup,        // nur noch aus Gründen der Sicherheit
                                    Semester = semester
                                };

                                semester.Groups.Add(semGroup);
                                Db.SemesterGroups.Add(semGroup);
                            }
                        }
                    }
                }
            }

            Db.SaveChanges();


            return RedirectToAction("Index", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCourse(Guid id)
        {
            var timeTableService = new TimeTableInfoService(Db);

            timeTableService.DeleteCourse(id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("NoGroups");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LockCourses(Guid id)
        {
            var semester = GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(s => s.Semester.Id == id))
                .ToList();
            allGroups.ForEach(x => x.IsInternal = true);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnLockCourses(Guid id)
        {
            var semester = GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(s => s.Semester.Id == id))
                .ToList();
            allGroups.ForEach(x => x.IsInternal = false);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}