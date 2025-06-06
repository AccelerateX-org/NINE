﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[CookieConsent]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Callback()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            var adminUser = UserManager.FindByName("admin");

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    FirstName = "Sys",
                    LastName = "Admin",
                };

                UserManager.Create(adminUser, "Pas1234?");
                var usr = UserManager.FindByName("admin");

                UserManager.AddToRole(usr.Id, "SysAdmin");
            }



            if (!Request.IsAuthenticated)
                return View("Landing");

            if (User.IsInRole("SysAdmin"))
                return RedirectToAction("Index", "Home", new {area = "Admin"});


            // Verteiler nach Rolle
            var user = GetCurrentUser();
            var student = GetCurrentStudent(user.Id);
            var members = MemberService.GetFacultyMemberships(user.Id);

            // Prio 1; Member
            if (members.Any())
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Prio 2; Student
            if (student != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Prio 3: Studiengangbewerber
            var cand = Db.Candidatures.Where(x => x.UserId.Equals(user.Id)).ToList();

            if (cand.Any())
            {
                return RedirectToAction("Index", "Candidature");
            }

            // Prio 4: noch nichts
            return View("FirstVisit");
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
                var activeOrgs = SemesterService.GetActiveOrganiser(semester);

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


        public ActionResult SelectFilter(string returnUrl)
        {
            var semName = Session["SemesterName"].ToString();

            ViewBag.CurrentSemnester = SemesterService.GetSemester(semName);
            ViewBag.ReturnUrl = returnUrl;

            var currentSemester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(currentSemester);

            // vom NextSemester 8 zurück

            var semesters = Db.Semesters.Where(x => x.StartCourses <= nextSemester.StartCourses)
                .OrderByDescending(x => x.EndCourses)
                .Take(5).ToList();


            return View(semesters);
        }


        public ActionResult SelectFilterConfirmed(Guid semId, string returnUrl)
        {
            var semester = SemesterService.GetSemester(semId);

            Session["SemesterName"] = semester.Name;
            Session["SemesterId"] = semester.Id;

            return RedirectToLocal(returnUrl);
        }

    }
}