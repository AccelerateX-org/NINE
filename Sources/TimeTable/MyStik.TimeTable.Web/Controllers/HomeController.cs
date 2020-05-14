using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[CookieConsent]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            return View("FirstVisit");

            /*
            var model = new HomeViewModel();

            var userDb = new ApplicationDbContext();

            model.Students = userDb.Users.Count(x => x.MemberState == MemberState.Student);
            model.Lecturers = userDb.Users.Count(x => x.MemberState == MemberState.Staff);
            model.Curricula = Db.Curricula.Count();
            model.Rooms = Db.Rooms.Count();

            return View("Landing", model);
            */
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult UniversityCalendar()
        {
            var model = new HomeViewModel();


            // Alle Semester mit veröffentlichten Semestergruppen
            var allPublishedSemester = Db.Semesters.Where(x => x.Groups.Any(g => g.IsAvailable)).OrderByDescending(s => s.EndCourses).Take(4).ToList();
            foreach (var semester in allPublishedSemester)
            {
                var activeOrgs = SemesterService.GetActiveOrganiser(semester, true);

                var semModel = new SemesterActiveViewModel
                {
                    Semester = semester,
                    Organisers = activeOrgs.ToList()
                };

                model.ActiveSemester.Add(semModel);
            }

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PrivacyStatement()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Contact(string email)
        {
            var model = new ContactMailModel
            {
                Email = email
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Contact(ContactMailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Send(model.Email, "hinz@hm.edu", "[fillter] " + model.Subject, model.Body);

            return View("ThankYou");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Missing()
        {
            return View();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Imprint()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult TestLab()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Fillter()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult TermsOfUse()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Semester(Guid id)
        {
            var semester = SemesterService.GetSemester(id);

            var curricula = SemesterService.GetActiveCurricula(semester, true).ToList();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Curricula = curricula.OrderBy(x => x.Organiser.ShortName).ThenBy(x => x.Name).ToList()
            };

            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Dictionary(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var courses = SemesterService.GetCourses(semester, curr);

            var model = new SemesterScheduleViewModel
            {
                Semester = semester,
                Curriculum = curr
            };

            foreach (var course in courses.OrderBy(x => x.Name))
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                var cModel = new CourseSummaryModel
                {
                    Course = course,
                    Lecturers = lectures
                };

                model.Courses.Add(cModel);
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Course(Guid semId, Guid currId, Guid courseId)
        {
            var semester = SemesterService.GetSemester(semId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);

            var model = new CourseScheduleViewModel
            {
                Semester = semester,
                Curriculum = curr,
                Course = course
            };

            return View(model);
        }

    }
}